using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CGDP2.Utilities;

namespace CGDP2.UtilityComponents
{
	/// <summary>
	///		An Array2D of Components attached to cell Transforms in row Transforms in the hierarchy.
	///		
	///		It is the user's responsibility to call
	///		<see cref="InitialiseArray"/>
	///		or <see cref="EnsureArrayInitialised"/>
	///		before using the class members.
	/// </summary>
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="25/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public class ComponentArray2D : MonoBehaviour
	{
		private ComponentArray2DRow[] m_rows;
		public IReadOnlyList<ComponentArray2DRow> rows => m_rows;
		public ComponentArray2DRow TryGetRow(int row) => m_rows.TryGetClassNullable(row);

		private Array2DLinearListWrapper<ComponentArray2DCell> m_cells;
		public IReadOnlyArray2DLinearListWrapper<ComponentArray2DCell> cells => m_cells;
		public ComponentArray2DCell TryGetCell(Coord coord) => cells.TryGetValueNullableClass(coord);

		private bool m_isInitialised = false;
		public bool isInitialised => m_isInitialised;

		public bool EnsureArrayInitialised()
		{
			if (m_isInitialised)
				return false;

			InitialiseArray();
			return true;
		}

		public void InitialiseArray()
		{
			if (m_isInitialised)
			{
				Debug.LogError("already initialised", this);
				return;
			}

			m_isInitialised = true;

			InitialiseRows();
			InvokeArrayInitialised();
			InvokeArrayInitialisedLate();
		}

		private void InitialiseRows()
		{
			var numRows = transform.childCount;
			m_rows = new ComponentArray2DRow[numRows];
			foreach (var (i, child) in transform.Cast<Transform>().IndexedElements())
			{
				var row = child.GetComponent<ComponentArray2DRow>();
				if (!row)
				{
					Debug.LogError("Child " + i + " of this "
						+ nameof(ComponentArray2D) + " does not have a "
						+ nameof(ComponentArray2DRow) + " component",
						this);
					continue;
				}
				m_rows[i] = row;
				row.ComponentArray2D_Init(this, i);
			}

			var numColumns = m_rows.AnyAsNullable()?.Max(row => row.cells.Count) ?? 0;
			var size = new Coord(column: numColumns, row: numRows);
			m_cells = new Array2DLinearListWrapper<ComponentArray2DCell>(size);
			foreach (var (irow, row) in m_rows.IndexedElements())
			{
				if (!row) continue;

				if (row.cells.Count != numColumns)
				{
					Debug.LogError("The number of columns in Row " + irow + " does not match the 2D array.", row);
				}

				foreach (var (column, cell) in row.cells.IndexedElements())
				{
					if (!cell) continue;
					m_cells[column: column, row: irow] = cell;
				}
			}
		}

		protected virtual void OnArrayInitialised() { }
		private void InvokeArrayInitialised()
		{
			this.TryElseLog(OnArrayInitialised);

			foreach (var row in rows)
			{
				if (!row) continue;
				row.ComponentArray2D_OnArrayInitialised();
			}
		}

		protected virtual void OnArrayInitialisedLate() { }
		private void InvokeArrayInitialisedLate()
		{
			foreach (var row in rows)
			{
				if (!row) continue;
				row.ComponentArray2D_OnArrayInitialisedLate();
			}

			this.TryElseLog(OnArrayInitialisedLate);
		}
	}
}
