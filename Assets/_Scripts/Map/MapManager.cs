using System.Collections.Generic;
using UnityEngine;


public class Coordinates {

    public int x;
    public int y;

    public Coordinates() {
    }

    public Coordinates(int _x, int _y) {
        x = _x;
        y = _y;
    }

    public int HexX {
        get { return y - x / 2; }
    }

    public int HexY {
        get { return x; }
    }

    public int HexZ {
        get { return -HexX - HexY; }
    }

    public override bool Equals(object obj) {
        var your_class = (Coordinates)obj;
        return your_class.x == this.x && your_class.y == this.y;
    }

    public override int GetHashCode() {
        int id_hashcode = x.GetHashCode();
        int name_hashcode = y.GetHashCode();
        return id_hashcode + name_hashcode;
    }

    public override string ToString() {
        return "Coord(" + x + ", " + y + ")" + " Hex(" + HexX + ", " + HexY + ", " + HexZ + ")";
    }
}

public class MapManager {
    static MapManager msMapManager = null;

    public int mMapCorrdinateWidth = 200;
    public int mMapCorrdinateHeight = 200;

    public int mSideLength = 1;

    public int[,] mMapDatas = null;

    public static MapManager GetInstance() {
        if (msMapManager == null) {
            msMapManager = new MapManager();
        }
        return msMapManager;
    }

    private MapManager() {
        Init();
    }
    public void Init() {
        mMapDatas = new int[mMapCorrdinateWidth, mMapCorrdinateHeight];
    }

    public bool CheckBoundary(Coordinates c) {
        return CheckBoundary(c.x, c.y);
    }

    bool CheckBoundary(int x, int y) {
        if (x >= 0 && x < mMapCorrdinateWidth && y >= 0 && y < mMapCorrdinateHeight) {
            return true;
        }
        return false;
    }

    // 坐标转换函数
    public Coordinates TerrainPositionToCorrdinate(Vector3 position) {
        Coordinates c = new Coordinates();
        c.x = (int)position.x;
        if (c.x % 2 == 0) {
            c.y = (int)(position.z + 0.5f);
        } else {
            c.y = (int)(position.z) + 1;
        }
        c.y = mMapCorrdinateHeight - c.y;
        return c;
    }

    public Vector3 CorrdinateToTerrainPosition(Coordinates c) {
        Vector3 position = new Vector3();
        position.x = c.x + 0.5f;
        if (c.x % 2 == 0) {
            position.z = mMapCorrdinateHeight - c.y;
        } else {
            position.z = mMapCorrdinateHeight - c.y - 0.5f;
        }
        return position;
    }

    public Vector3 TerrainPositionToCorrdinatePosition(Vector3 terrainPosition) {
        return CorrdinateToTerrainPosition(TerrainPositionToCorrdinate(terrainPosition));
    }

    public List<Coordinates> GetNeighbours(Vector3 position) {
        return GetNeighbours(TerrainPositionToCorrdinate(position));
    }

    public List<Coordinates> GetNeighbours(Coordinates c) {
        List<Coordinates> neightbors = new List<Coordinates>();
        neightbors.Add(new Coordinates(c.x, c.y - 1));
        neightbors.Add(new Coordinates(c.x, c.y + 1));
        if (c.x % 2 == 0) {
            neightbors.Add(new Coordinates(c.x - 1, c.y - 1));
            neightbors.Add(new Coordinates(c.x - 1, c.y));
            neightbors.Add(new Coordinates(c.x + 1, c.y - 1));
            neightbors.Add(new Coordinates(c.x + 1, c.y));
        } else {
            neightbors.Add(new Coordinates(c.x - 1, c.y));
            neightbors.Add(new Coordinates(c.x - 1, c.y + 1));
            neightbors.Add(new Coordinates(c.x + 1, c.y));
            neightbors.Add(new Coordinates(c.x + 1, c.y + 1));
        }
        return neightbors;
    }



}
