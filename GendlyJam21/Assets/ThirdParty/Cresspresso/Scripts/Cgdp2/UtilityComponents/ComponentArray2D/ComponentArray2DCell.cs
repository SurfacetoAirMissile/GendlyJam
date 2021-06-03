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
	public class ComponentArray2DCell : MonoBehaviour
	{
		private ComponentArray2D m_array;
		public ComponentArray2D array => m_array;

		private ComponentArray2DRow m_row;
		public ComponentArray2DRow row => m_row;

		private Coord m_coord = new Coord(-1, -1);
		public Coord coord => m_coord;

		private bool m_isInitialised = false;
		public bool isInitialised => m_isInitialised;

		/// <summary>
		/// Called by <see cref="ComponentArray2DRow"/>.
		/// </summary>
		internal void ComponentArray2DRow_Init(ComponentArray2DRow row, Coord coord)
		{
			if (m_isInitialised)
			{
				Debug.LogError("already initialised", this);
				return;
			}

			m_isInitialised = true;

			m_row = row;
			m_coord = coord;
			m_array = row.array;

			name += $" [Cell column: {coord.column}, row: {coord.row}]";
		}

		protected virtual void OnArrayInitialised() { }
		internal void ComponentArray2DRow_OnArrayInitialised() => this.TryElseLog(OnArrayInitialised);

		protected virtual void OnArrayInitialisedLate() { }
		internal void ComponentArray2DRow_OnArrayInitialisedLate() => this.TryElseLog(OnArrayInitialisedLate);
	}
}
