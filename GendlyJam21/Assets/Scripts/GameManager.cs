using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour
{
    public interface IReadOnlyTileGameInfo
    {
        bool canPlace { get; }
        bool canEnemiesWalk { get; }
    }

    [System.Serializable]
    private class TileGameInfo : IReadOnlyTileGameInfo
    {
        public bool canPlace;
        public bool canEnemiesWalk;

        bool IReadOnlyTileGameInfo.canPlace => this.canPlace;
        bool IReadOnlyTileGameInfo.canEnemiesWalk => this.canEnemiesWalk;
    }



    private Dictionary<Sprite, TileGameInfo> tileData;

    /// <returns><see langword="null"/> if the sprite is not in the dictionary.</returns>
    public IReadOnlyTileGameInfo TryGetTileData(Sprite tileSprite)
    {
        if (!tileSprite)
        { return null; }
        return tileData.TryGetValueNullable(tileSprite);
    }



    public static GameManager Instance { get; private set; } = null;

    private float globalTimer = 0f;
    private GameObject heldObject = null;
    private SpriteRenderer heldObjectRenderer = null;
    //private

    [Header("Setup")]
    [SerializeField]
    private Tilemap tilemap = default;
    [SerializeField]
    private GridLayout grid = default;
    [SerializeField]
    private Sprite[] sprites = default;
    [SerializeField]
    private TileGameInfo[] info = default;
    [SerializeField]
    private float generation = default;
    [SerializeField]
    private float credits = default;
    [Header("Text")]
    [SerializeField]
    private TMP_Text powerText = default;
    [SerializeField]
    private TMP_Text creditsText = default;
    [Header("Colours")]
    [SerializeField]
    private Color canPlaceTint = default;
    [SerializeField]
    private Color cannotPlaceTint = default;
    [SerializeField]
    private Color cannotAffordTint = default;
    [Header("Prefabs")]
    [SerializeField]
    private GameObject powerObject = default;
    [SerializeField]
    private GameObject basicTowerPrefab = default;



    private void Awake()
    {
        // if there isn't already an instance
        if (!Instance)
        {
            // declare oneself as thine instance
            Instance = this;

            // setup the tile dictionary
            tileData = new Dictionary<Sprite, TileGameInfo>();
            for (int i = 0; i < sprites.Length; i++)
            {
                tileData.Add(sprites[i], info[i]);
            }

            UpdateGeneration(0f);
            UpdateCredits(0);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if an object is held...
        if (heldObject)
        {
            // move the held object to the mouse
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newPos.z = 0f;
            heldObject.transform.position = newPos;

            bool tintRed = true;

            // get the tile
            Vector3Int gridPosition = grid.WorldToCell(newPos);
            TileGameInfo hoverTileInfo = tileData[tilemap.GetSprite(gridPosition)];

            // if the tile can be placed on...
            if (hoverTileInfo.canPlace)
            {
                // if there was no result testing the tower layer...
                RaycastHit2D raycastResult = Physics2D.Raycast(newPos, -Vector2.up, 0.01f, LayerMask.GetMask("Tower"));
                if (!raycastResult)
                {
                    CostComponent costComp = heldObject.GetComponent<CostComponent>();

                    // tint the tower green
                    heldObjectRenderer.color = canPlaceTint;
                    // I'm using cell to world after doing world to cell to get the exact position of the cell, so that the tower is placed on the center
                    heldObject.transform.position = tilemap.GetCellCenterWorld(gridPosition);
                    tintRed = false;

                    if (CanAfford(costComp.price))
                    {
                        // if the left mouse button is pressed...
                        if (Input.GetMouseButtonDown(0))
                        {

                            // get component call happens only once.
                            var tower = heldObject.GetComponent<ATower>();
                            tower.OnPlacement();
                            tower.OnCastlePowerChanged(CheckPower());
                            // if the tower has a cost component
                            if (costComp)
                            {
                                // deduct the cost
                                costComp.OnPlacement();
                            }
                            heldObject.GetComponent<BoxCollider2D>().enabled = true;
                            // put the color back to white
                            heldObjectRenderer.color = Color.white;
                            SetHeldObject(null);
                        }
                    }
                    else
                    {
                        heldObjectRenderer.color = cannotAffordTint;
                        // I'm using cell to world after doing world to cell to get the exact position of the cell, so that the tower is placed on the center
                        heldObject.transform.position = tilemap.GetCellCenterWorld(gridPosition);
                        tintRed = false;
                    }
                    
                    
                }
                
            }
            if (tintRed)
            {
                // tint the tower red
                heldObjectRenderer.color = cannotPlaceTint;
            }
        }

        globalTimer += Time.deltaTime;
    }

    private void SetHeldObject(GameObject _object)
    {
        heldObject = _object;
        if (_object)
        {
            heldObjectRenderer = _object.GetComponent<SpriteRenderer>();
        }
        else
        {
            heldObjectRenderer = null;
        }
    }

    public bool CheckPower()
    {
        return generation >= 0f;
    }

    /// <changelog>
    ///     <log date="04/06/2020" time="10:09" author="Elijah Shadbolt">
    ///         Call all towers OnPowerChanged event.
    ///     </log>
    /// </changelog>
    public void UpdateGeneration(float _change)
    {
        generation += _change;
        powerText.text = generation.ToString("0") + " MW";

        // Invoke the OnCastlePowerChanged event on all Tower instances.
        bool isPowered = CheckPower();
        foreach (var tower in ATower.Instances)
        {
            tower.OnCastlePowerChanged(isPowered);
        }
    }

    // referenced by a button in the demo scene
    public void HoldTower(int _tower)
    {

        // TODO generalize function, to avoid lots of duplicate lines in the future

        switch (_tower)
        {
            // power generation
            default:
            case 0:
                HoldTower(this.powerObject);
                return;
            case 1:
                HoldTower(this.basicTowerPrefab);
                return;
        }
    }

    /// <changelog>
    ///     <log date="04/06/2020" time="10:01" author="Elijah Shadbolt">
    ///         Added this function to hold the repeated stuff in <see cref="HoldTower(int)"/>.
    ///     </log>
    /// </changelog>
    private void HoldTower(GameObject _towerPrefab)
    {
        if (heldObject)
        {
            // if the held object is a clone of the button being pressed (powerObject corellates to 0) destroy it (cancel placing)
            if (heldObject.name.Contains(_towerPrefab.name))
            {
                Destroy(heldObject);
                SetHeldObject(null);
                return;
            }
            else // if there is a held object but it isn't the same tower, destroy the held object and set the power object to be the new one.
            {
                Destroy(heldObject);
                SetHeldObject(null);
            }
        }
        // in any other case, like when there isn't a held object or when you're switching towers to place, set the held object to be the power object.
        SetHeldObject(Instantiate(_towerPrefab, TowerParentSingleton.Instance.theParent));
    }

    public void UpdateCredits(int _credits)
    {
        credits += _credits;
        creditsText.text = "$" + credits.ToString("0");
    }

    public bool CanAfford(int _cost)
    {
        return credits >= _cost;
    }

    private void OnDestroy()
    {
        HandleTextFile.WriteLine("\n" + globalTimer.ToString("0.00") + " seconds!");
        Instance = null;

    }
}
