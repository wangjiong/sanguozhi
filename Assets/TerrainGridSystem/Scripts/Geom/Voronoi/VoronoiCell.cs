using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TGS.Geom {
	public class VoronoiCell {
		public List <Segment> segments;
		public Point center;
		public List<Point>top, left, bottom, right; // for cropping

		public VoronoiCell (Point center) {
			segments = new List<Segment> (16);
			this.center = center;
			left = new List<Point> ();
			top = new List<Point> ();
			bottom = new List<Point> ();
			right = new List<Point> ();
		}

		public Polygon GetPolygon (int edgeSubdivisions, float curvature) {
			Connector connector = new Connector ();
			for (int k=0; k<segments.Count; k++) {
				Segment s = segments [k];
				if (!s.deleted) {
					if (edgeSubdivisions>1) {
						connector.AddRange (s.Subdivide(edgeSubdivisions, curvature));
					} else {
						connector.Add (s);
					}
				}
			}
			return connector.ToPolygonFromLargestLineStrip ();
		}

	
		public Point centroid {
			get {
				Point point = Point.zero;
				int count=0;
				for (int k=0;k<segments.Count;k++) {
					Segment s = segments[k];
					if (!s.deleted) {
						point += segments[k].start;
						point += segments[k].end;
						count+=2;
					}
				}
				if (count>0) point /= count;
				return point;
			}
		}

	}

}
