using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="25/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public class ComponentArray2DRow : MonoBehaviour
	{
		private ComponentArray2D m_array;
		public ComponentArray2D array => m_array;

		private int m_rowIndex = -1;
		public int rowIndex => m_rowIndex;

		private ComponentArray2DCell[] m_cells;
		public IReadOnlyList<ComponentArray2DCell> cells => m_cells;
		public ComponentArray2DCell TryGetCell(int column) => m_cells.TryGetClassNullable(column);

		private bool m_isInitialised = false;
		public bool isInitialised => m_isInitialised;

		/// <summary>
		/// Called by <see cref="ComponentArray2D"/>.
		/// </summary>
		internal void ComponentArray2D_Init(ComponentArray2D array, int rowIndex)
		{
			if (m_isInitialised)
			{
				Debug.LogError("already initialised", this);
				return;
			}

			m_isInitialised = true;

			m_array = array;
			m_rowIndex = rowIndex;

			name += $" [Row {rowIndex}]";

			InitialiseCells();
		}

		private void InitialiseCells()
		{
			m_cells = new ComponentArray2DCell[transform.childCount];
			foreach (var (i, child) in transform.Cast<Transform>().IndexedElements())
			{
				var cell = child.GetComponent<ComponentArray2DCell>();
				if (!cell)
				{
					Debug.LogError("Child " + i + " of this "
						+ nameof(ComponentArray2DRow) + " does not have a "
						+ nameof(ComponentArray2DCell) + " component",
						this);
					continue;
				}
				m_cells[i] = cell;
				var coord = new Coord(column: i, row: rowIndex);
				cell.ComponentArray2DRow_Init(this, coord);
			}
		}

		protected virtual void OnArrayInitialised() { }
		internal void ComponentArray2D_OnArrayInitialised()
		{
			this.TryElseLog(OnArrayInitialised);

			foreach (var cell in cells)
			{
				if (!cell) continue;
				cell.ComponentArray2DRow_OnArrayInitialised();
			}
		}

		protected virtual void OnArrayInitialisedLate() { }
		internal void ComponentArray2D_OnArrayInitialisedLate()
		{
			foreach (var cell in cells)
			{
				if (!cell) continue;
				cell.ComponentArray2DRow_OnArrayInitialisedLate();
			}

			this.TryElseLog(OnArrayInitialisedLate);
		}
	}
}
