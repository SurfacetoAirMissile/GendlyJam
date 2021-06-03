using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    private class TileGameInfo
    {
        public bool canPlace;
    }



    private Dictionary<Sprite, TileGameInfo> tileData;

    public static GameManager Instance { get; private set; } = null;


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
                RaycastHit2D raycastResult = Physics2D.Raycast(newPos, -Vector2.up, 1f, LayerMask.GetMask("Tower"));
                if (!raycastResult)
                {
                    // tint the tower red
                    heldObjectRenderer.color = canPlaceTint;
                    // I'm using cell to world after doing world to cell to get the exact position of the cell, so that the tower is placed on the center
                    heldObject.transform.position = tilemap.GetCellCenterWorld(gridPosition);
                    tintRed = false;

                    // if the left mouse button is pressed...
                    if (Input.GetMouseButtonDown(0))
                    {
                        // get component call happens only once.
                        var tower = heldObject.GetComponent<ATower>();
                        tower.OnPlacement();
                        tower.OnCastlePowerChanged(CheckPower());
                        heldObject.GetComponent<BoxCollider2D>().enabled = true;
                        // put the color back to white
                        heldObjectRenderer.color = Color.white;
                        SetHeldObject(null);
                    }
                }
                
            }
            if (tintRed)
            {
                // tint the tower red
                heldObjectRenderer.color = cannotPlaceTint;
            }
        }
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
        return generation > 0f;
    }

    /// <changelog>
    ///     <log date="04/06/2020" time="10:09" author="Elijah Shadbolt">
    ///         Call all towers OnPowerChanged event.
    ///     </log>
    /// </changelog>
    public void UpdateGeneration(float _change)
    {
        generation += _change;
        powerText.text = generation.ToString("0");

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
        // set the held object to a new copy of the power object
        if (heldObject)
        {
            if (heldObject.name.Contains(_towerPrefab.name))
            {
                Destroy(heldObject);
                SetHeldObject(null);
                return;
            }
        }
        SetHeldObject(Instantiate(_towerPrefab, TowerParentSingleton.Instance.theParent));
    }

    public void DeductCredits(int _credits)
    {
        credits -= _credits;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
