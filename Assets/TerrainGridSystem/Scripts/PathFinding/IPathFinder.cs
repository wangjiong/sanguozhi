using System;
using System.Collections.Generic;
using System.Text;

namespace TGS.PathFinding {

	public delegate int OnCellCross(int location);

	interface IPathFinder {

		bool Stopped {
			get;
		}

		HeuristicFormula Formula {
			get;
			set;
		}

		bool Diagonals {
			get;
			set;
		}

		bool HeavyDiagonals {
			get;
			set;
		}

		bool HexagonalGrid {
			get;
			set;
		}

		int HeuristicEstimate {
			get;
			set;
		}

//		bool PunishChangeDirection {
//			get;
//			set;
//		}
//
//		bool TieBreaker {
//			get;
//			set;
//		}

		int SearchLimit {
			get;
			set;
		}

//		float CompletedTime {
//			get;
//			set;
//		}

		void FindPathStop ();

		List<PathFinderNode> FindPath (PathFindingPoint start, PathFindingPoint end);
		void SetCalcMatrix(byte[] grid);

		OnCellCross OnCellCross { get; set; }

	}
}
