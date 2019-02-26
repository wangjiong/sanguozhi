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

    public Coordinates SetHexXY(int hexX, int hexY) {
        x = hexY;
        y = x / 2 + hexX;
        return this;
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

public enum TerrainType {
    TerrainType_Invalid, // 无效地形
    TerrainType_Caodi, // 草地
    TerrainType_Tu,// 土
    TerrainType_Shadi,// 沙地
    TerrainType_Shidi,// 湿地
    TerrainType_Duquan,// 毒泉
    TerrainType_Sen,// 森
    TerrainType_Chuan,// 川
    TerrainType_He,// 河
    TerrainType_Hai,// 海
    TerrainType_Huangdi,// 荒地
    TerrainType_Zhujing,// 主径
    TerrainType_Zhandao,//栈道
    TerrainType_Dusuo,// 渡所
    TerrainType_Qiantan,// 浅滩
    TerrainType_An,// 岸
    TerrainType_Ya,// 涯
    TerrainType_Dushi,// 都市
    TerrainType_Guansuo,// 关所
    TerrainType_Gang,// 港
    TerrainType_Xiaojing, //小径


    // 00000000 00000000 00000000 00000000 ， 后面8位代表地表地形的类型 ， 前面的24位代表复合类型
    TerrainType_Wujiang = 1 << 8,// 武将
    TerrainType_Kaifajianzhu = 1 << 9, // 开发建筑
    TerrainType_Fire =    1 << 10,// 火
    TerrainType_Junshisheshi = 1 << 11,// 军事设施
}



public class MapManager {
    static MapManager msMapManager = null;

    public int mMapCorrdinateWidth = 200;
    public int mMapCorrdinateHeight = 200;

    static uint TERRAINTYPE_MASK = 0xFFFFFF00; // 后面8位代表地表地形的类型 ， 前面的24位代表复合类型
    public static uint ToLowTerrainType(uint terrainType) {
        return terrainType & ~TERRAINTYPE_MASK;
    }

    public int mSideLength = 1;

    uint[,] mMapDatas = null;

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
        mMapDatas = new uint[mMapCorrdinateWidth, mMapCorrdinateHeight];
    }

    public uint[,] GetMapDatas() {
        return mMapDatas;
    }

    public uint GetTerrainType(Coordinates coordinates) {
        return mMapDatas[coordinates.x, coordinates.y];
    }

    public void AddTerrainType(Coordinates coordinates, TerrainType terrainType) {
        uint originTerrainType = mMapDatas[coordinates.x, coordinates.y];
        if ((uint)terrainType < 256) {
            // 1.地表地形的类型，只设置低8位
            mMapDatas[coordinates.x, coordinates.y] = (originTerrainType & TERRAINTYPE_MASK) + (uint)terrainType;
        } else {
            // 2.复合类型，只设置高24位
            mMapDatas[coordinates.x, coordinates.y] = originTerrainType | (uint)terrainType;
        }
    }

    public void RemoveTerrainType(Coordinates coordinates, TerrainType terrainType) {
        uint originTerrainType = mMapDatas[coordinates.x, coordinates.y];
        if ((uint)terrainType < 256) {
            // 1.地表地形的类型，只设置低8位
            mMapDatas[coordinates.x, coordinates.y] = originTerrainType & TERRAINTYPE_MASK;
        } else {
            // 2.复合类型，只设置高24位
            mMapDatas[coordinates.x, coordinates.y] = originTerrainType & ~(uint)terrainType;
        }
    }

    public bool ContainTerrainType(Coordinates coordinates, TerrainType terrainType) {
        uint originTerrainType = mMapDatas[coordinates.x, coordinates.y];
        if ((uint)terrainType < 256) {
            // 1.地表地形的类型，只设置低8位
            if (mMapDatas[coordinates.x, coordinates.y] == (originTerrainType & TERRAINTYPE_MASK)) {
                return true;
            };
        } else {
            // 2.复合类型，只设置高24位
            if ((mMapDatas[coordinates.x, coordinates.y] & (uint)terrainType) != 0) {
                return true;
            }
        }
        return false;
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

    public Vector3 TerrainPositionToCenterPosition(Vector3 terrainPosition) {
        return CorrdinateToTerrainPosition(TerrainPositionToCorrdinate(terrainPosition));
    }

    public List<Coordinates> GetNeighbours(Vector3 position) {
        return GetNeighbours(TerrainPositionToCorrdinate(position));
    }

    public List<Coordinates> GetNeighbours(Coordinates c) {
        List<Coordinates> neighbours = new List<Coordinates>();
        neighbours.Add(new Coordinates().SetHexXY(c.HexX - 1, c.HexY));
        neighbours.Add(new Coordinates().SetHexXY(c.HexX + 1, c.HexY));
        neighbours.Add(new Coordinates().SetHexXY(c.HexX, c.HexY - 1));
        neighbours.Add(new Coordinates().SetHexXY(c.HexX, c.HexY + 1));
        neighbours.Add(new Coordinates().SetHexXY(c.HexX - 1, c.HexY + 1));
        neighbours.Add(new Coordinates().SetHexXY(c.HexX + 1, c.HexY - 1));
        return neighbours;
    }

    public List<Coordinates> GetAllAroundN(Coordinates c, int n) {
        List<Coordinates> neighbours = new List<Coordinates>();
        // 所有的圈
        for (int i = 0; i <= n; i++) {
            GetAroundN(neighbours, c, i);
        }
        return neighbours;
    }

    public void GetAroundN(List<Coordinates> neighbours, Coordinates c, int n) {
        if (n == 0) {
            neighbours.Add(c);
            return;
        }
        int hexX = c.HexX;
        int hexY = c.HexY;
        AddHexNode(hexX - n, hexY, neighbours);        // N
        AddHexNode(hexX + n, hexY, neighbours);// S
        AddHexNode(hexX, hexY - n, neighbours);    // NW
        AddHexNode(hexX, hexY + n, neighbours);// SE
        AddHexNode(hexX - n, hexY + n, neighbours);    // NE
        AddHexNode(hexX + n, hexY - n, neighbours);    // SW
        for (int i = 1; i < n; i++) {
            // W
            AddHexNode(hexX + i, hexY - n, neighbours);
            // E
            AddHexNode(hexX - n + i, hexY + n, neighbours);
            // NW
            AddHexNode(hexX - n + i, hexY - i, neighbours);
            // SE
            AddHexNode(hexX + i, hexY + n - i, neighbours);
            // NE
            AddHexNode(hexX - n, hexY + i, neighbours);
            // SW
            AddHexNode(hexX + n, hexY - n + i, neighbours);
        }
    }

    public void AddHexNode(int hexX, int hexY, List<Coordinates> neighbours) {
        Coordinates c = new Coordinates().SetHexXY(hexX, hexY);
        neighbours.Add(c);
    }
}