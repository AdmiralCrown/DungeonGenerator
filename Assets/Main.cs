using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DungeonGeneratorClasses;
using System.IO;
using System.Text;
using System;


public class Main : MonoBehaviour
{
    //RoomType Definition
    //#####################################################################################################
    void initializeRoomTypes()
    {


        roomTypes.Add(new Room("empty", new Color(0, 0, 0), 100.0, ' '));
        roomTypes.Add(new Room("entrance", new Color(255, 0, 255), 100.0, 'x'));
        roomTypes.Add(new Room("corridor", new Color(0, 200, 255), 100.0, 'o'));
        roomTypes.Add(new Room("room", new Color(0, 255, 0), 100.0, '◙'));
        roomTypes.Add(new Room("boss", new Color(200, 100, 5), 'b'));

        
    }
    //#####################################################################################################

    // Variables
    //#####################################################################################################

    #region Variables

    const int xDimension = 40, yDimension = 40;
    const int xTiles = 3, yTiles = 3;

    RoomTile[,] roomTiles;

    RoomTile actTile;

    List<Room> roomTypes = new List<Room>();

    List<roomDimension> createdRooms = new List<roomDimension>();

    nextDir nextDirection;

    int corridorNr = 0, roomNr = 0, mapNr = 0;

    bool alwaysTrue = true;

    OpMode operationMode = OpMode.ClickOnly;
    //OPERATION MODE

    public string openMapFileNr;


    static RTP[,] room_full = new RTP[3, 3] { { RTP.Room, RTP.Room, RTP.Room }, { RTP.Room, RTP.Room, RTP.Room }, { RTP.Room, RTP.Room, RTP.Room } }, // only room
           corridor_straight = new RTP[3, 3] { { RTP.Wall, RTP.Corridor, RTP.Wall }, { RTP.Wall, RTP.Corridor, RTP.Wall }, { RTP.Wall, RTP.Corridor, RTP.Wall } }, //straight corridor 
           corridor_curve = new RTP[3, 3] { { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Corridor, RTP.Corridor, RTP.Wall }, { RTP.Wall, RTP.Corridor, RTP.Wall } }; // left corridor
    #endregion

    //#####################################################################################################

    // Initialization
    //#####################################################################################################
    void Start()
    {

        transform.position = new Vector3(xDimension / 8.0f, yDimension / 8.0f, -(xDimension + xDimension));
        roomTiles = new RoomTile[xDimension, yDimension];
        initializeRoomTypes();
        initializeRooms();

        switch (operationMode) {

            case OpMode.ClickOnly:
            case OpMode.AutomaticMode:

                nextDirection = new nextDir(10 + UnityEngine.Random.Range(0, xDimension / 2), 10 + UnityEngine.Random.Range(0, yDimension / 2), Dir.Count);
                setRoomType("entrance", nextDirection.XNext, nextDirection.YNext);

                //print ("x: " + nextWay [0] + " y: " + nextWay [1] + " emptyness: " + roomTiles [nextWay [0], nextWay [1]].Emptyness);

                break;

            case OpMode.LoadMap:

                //TODO
                break;
            default:
                break;
        }


    }
    //#####################################################################################################

    // Update is called once per frame
    //#####################################################################################################
    void Update()
    {
        switch (operationMode) {

            //################################################################################################
            case OpMode.ClickOnly:

                nextDirection = simplePath(); //SIMPLE MAP GENERATION
                if (corridorNr > 5 + UnityEngine.Random.Range(3, 5)) {
                    nextDirection = simpleRoom();
                    corridorNr = 0;
                }
                corridorNr++;

                //if (roomNr >= 5)
                //    roomNr = 0;

                if (Input.GetKeyDown(KeyCode.T)) //TEST
                {
                    //printRoomTileParts(RotateMatrix(corridor_curve));
                    //RotateMatrix(corridor_curve);
                    //printRoomTileParts(corridor_curve);
                    visualizeMap();
                }
                   
                if (Input.GetKeyDown(KeyCode.S)) //MAPSCORE
                    print("Map Score: " + getMapScore());

                if (Input.GetMouseButtonDown(1)) //SAVE
                    saveMap("Map", ".txt");

                if (Input.GetMouseButtonDown(0)) {//RESTART
                    restart();
                }

                //setRoom (createdRooms [createdRooms.Count - 1], "boss");
                break;
            //################################################################################################
            case OpMode.AutomaticMode:

                restart();

                while (alwaysTrue) {

                    nextDirection = simplePath(); //SIMPLE MAP GENERATION
                    if (corridorNr > 5 + UnityEngine.Random.Range(3, 5)) {
                        nextDirection = simpleRoom();
                        corridorNr = 0;
                    }
                    corridorNr++;

                    if (roomNr >= 5)
                        roomNr = 0;

                    if (alwaysTrue == false) {
                        int actMapScore = (int)getMapScore();

                        mapNr++;

                        if (actMapScore > 63000) {
                            saveMap("Map_" + mapNr.ToString(), ".txt");
                            print("Gefunden! Map Score: " + actMapScore);
                        }
                        print("Map NR: " + mapNr + " MapScore: " + actMapScore);
                    }
                }

                alwaysTrue = true;

                break;

            //################################################################################################
            case OpMode.LoadMap:
                if (Input.GetMouseButtonDown(0)) {
                    restart();
                    loadMap("Map_" + openMapFileNr, ".txt");
                }
                if (Input.GetMouseButtonDown(1))
                    print("MapScore: " + (int)getMapScore());

                break;
            default:
                break;
        }

    }
    //#####################################################################################################

    void restart()
    {
        //roomNr = 0;
        corridorNr = 0;
        destroyAll();
        Start();
    }

    void destroyAll()
    {

        foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
            if (o.tag != "MainCamera")
                Destroy(o);
    }

    double getMapScore()
    {
        double totalScore = 0;
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                totalScore += 100.0 - roomTiles[x, y].Emptyness;
            }
        }
        return totalScore;
    }

    void initializeRooms()
    {
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                actTile = new RoomTile(GameObject.CreatePrimitive(PrimitiveType.Cube));
                actTile.TileObject.transform.position = new Vector3(0.125f + x / 4.0f, 0.125f + y / 4.0f, 0);
                actTile.TileObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                roomTiles[x, y] = actTile;

            }
        }
    }

    void setRoomType(string roomType, int xPos, int yPos)
    {
        RoomTile roomToChange = roomTiles[xPos, yPos];

        Room roomTypeFound = roomTypes.Find(x => x.Type == roomType);

        overwriteRoom(roomTypeFound, roomToChange);

        setRoomColor(roomToChange);        
    }

    void setRoomConnectionsPath(Dir lastDirection, Dir direction, int xPos, int yPos)
    {
        RoomTile roomToChange = roomTiles[xPos, yPos];

        switch (lastDirection)
        {
            case Dir.Up:
                switch (direction)
                {
                    case Dir.Up:
                        roomToChange.RoomTileParts = RotateMatrix(corridor_straight);
                        break;
                    case Dir.Right:
                        roomToChange.RoomTileParts = corridor_curve;
                        break;
                    case Dir.Down:
                        //NOTHING
                        break;
                    case Dir.Left:
                        roomToChange.RoomTileParts = RotateMatrix(corridor_curve);
                        break;
                }
                break;
            case Dir.Right:
                switch (direction)
                {
                    case Dir.Up:
                        roomToChange.RoomTileParts = RotateMatrix(RotateMatrix(corridor_curve));
                        break;
                    case Dir.Right:
                        roomToChange.RoomTileParts = corridor_straight;
                        break;
                    case Dir.Down:
                        roomToChange.RoomTileParts = RotateMatrix(corridor_curve);
                        break;
                    case Dir.Left:
                        //NOTHING
                        break;
                }
                break;
            case Dir.Down:
                switch (direction)
                {
                    case Dir.Up:
                        //NOTHING
                        break;
                    case Dir.Right:
                        roomToChange.RoomTileParts = RotateMatrix(RotateMatrix(RotateMatrix(corridor_curve)));
                        break;
                    case Dir.Down:
                        roomToChange.RoomTileParts = RotateMatrix(corridor_straight);
                        break;
                    case Dir.Left:
                        roomToChange.RoomTileParts = RotateMatrix(RotateMatrix(corridor_curve));
                        break;
                }
                break;
            case Dir.Left:
                switch (direction)
                {
                    case Dir.Up:
                        roomToChange.RoomTileParts = RotateMatrix(RotateMatrix(RotateMatrix(corridor_curve)));
                        break;
                    case Dir.Right:
                        //NOTHING
                        break;
                    case Dir.Down:
                        roomToChange.RoomTileParts = corridor_curve;
                        break;
                    case Dir.Left:
                        roomToChange.RoomTileParts = corridor_straight;
                        break;
                }
                break;
        }

    }
    void setAllRoomConnections()
    {
        for (int x = 0; x < xDimension; x++)
        {
            for (int y = 0; y < yDimension; y++)
            {
                RoomTile roomToChange = roomTiles[x, y];
                switch (roomToChange.Type)
                {
                    case "empty":
                        break;

                    case "corridor":
                        break;
                    case "room":
                        break;

                    default:
                        break;
                }
            }
        }
    }
    void overwriteRoom(Room roomTypeFound, RoomTile roomToChange)
    {
        roomToChange.Type = roomTypeFound.Type;
        roomToChange.Color = roomTypeFound.Color;
        roomToChange.Symbol = roomTypeFound.Symbol;
    }


    void setRoomColor(RoomTile roomToChange)
    {
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = roomToChange.Color;

        roomToChange.TileObject.GetComponent<MeshRenderer>().material = newMaterial;
    }

    void setRoom(roomDimension rDim, string roomType)
    {
        for (int x = rDim.XRoomCornerMin; x <= rDim.XRoomCornerMax; x++) {
            for (int y = rDim.YRoomCornerMin; y <= rDim.YRoomCornerMax; y++) {
                setRoomType(roomType, x, y);
            }
        }
    }

    nextDir simpleRoom()
    {
        roomDimension rDim = spaceforRoom(UnityEngine.Random.Range(3, 7), UnityEngine.Random.Range(3, 7)); //ATM nur gerade

        nextDir roomExit = new nextDir();

        if (rDim.RoomPossible == true) {
            roomNr++;
            setRoom(rDim, "room");

            createdRooms.Add(rDim);

            calculateEmptyness();

            roomExit.XNext = rDim.XExit;
            roomExit.YNext = rDim.YExit;
            roomExit.NextDirection = roomExit.NextDirection;

            //print ("x: " + roomExit [0] + " y: " + roomExit [1] + " emptyness: " + roomTiles [roomExit [0], roomExit [1]].Emptyness);

            return roomExit;
        } else {
            roomExit = nextDirection;
            return roomExit;
        }

    }

    roomDimension spaceforRoom(int xRoomSize, int yRoomSize)
    {
        roomDimension rDim = new roomDimension();

        int xStart = nextDirection.XNext, yStart = nextDirection.YNext;

        switch (nextDirection.NextDirection) {
            case Dir.Up:
                rDim.XRoomCornerMin = xStart - ((xRoomSize - 1) / 2);
                rDim.XRoomCornerMax = xStart + ((xRoomSize - 1) / 2);
                rDim.YRoomCornerMin = yStart + 1;
                rDim.YRoomCornerMax = yStart + yRoomSize;
                rDim.XExit = xStart;
                rDim.YExit = rDim.YRoomCornerMax;
                break;
            case Dir.Right:
                rDim.XRoomCornerMin = xStart + 1;
                rDim.YRoomCornerMin = yStart - ((yRoomSize - 1) / 2);
                rDim.XRoomCornerMax = xStart + xRoomSize;
                rDim.YRoomCornerMax = yStart + ((yRoomSize - 1) / 2);
                rDim.XExit = rDim.XRoomCornerMax;
                rDim.YExit = yStart;
                break;
            case Dir.Down:
                rDim.YRoomCornerMin = yStart - yRoomSize;
                rDim.XRoomCornerMax = xStart + ((xRoomSize - 1) / 2);
                rDim.YRoomCornerMax = yStart - 1;
                rDim.XRoomCornerMin = xStart - ((xRoomSize - 1) / 2);
                rDim.XExit = xStart;
                rDim.YExit = rDim.YRoomCornerMin;
                break;
            case Dir.Left:
                rDim.XRoomCornerMin = xStart - xRoomSize;
                rDim.YRoomCornerMin = yStart - ((yRoomSize - 1) / 2);
                rDim.XRoomCornerMax = xStart - 1;
                rDim.YRoomCornerMax = yStart + ((yRoomSize - 1) / 2);
                rDim.XExit = rDim.XRoomCornerMin;
                rDim.YExit = yStart;
                break;
            default:
                rDim.XRoomCornerMin = 0;
                rDim.YRoomCornerMin = 0;
                rDim.XRoomCornerMax = -1;
                rDim.YRoomCornerMax = -1;
                rDim.XExit = nextDirection.XNext;
                rDim.YExit = nextDirection.YNext;
                break;
        }
        if (rDim.XRoomCornerMax >= xDimension || rDim.YRoomCornerMax >= yDimension || rDim.XRoomCornerMin < 0 || rDim.YRoomCornerMin < 0) {
            rDim.RoomPossible = false;
            return rDim;
        }
        if (rDim.XExit >= xDimension || rDim.YExit >= yDimension || rDim.XExit < 0 || rDim.YExit < 0) {
            rDim.RoomPossible = false;
            print("Exit corridor nicht moeglich!");
            return rDim;
        }
        for (int x = rDim.XRoomCornerMin; x <= rDim.XRoomCornerMax; x++) {
            for (int y = rDim.YRoomCornerMin; y <= rDim.YRoomCornerMax; y++) {
                if (roomTiles[x, y].Type != "empty")
                    rDim.RoomPossible = false;
            }
        }

        return rDim;
    }

    nextDir simplePath()
    {
        int xStart = nextDirection.XNext, yStart = nextDirection.YNext;

        Dir lastDirection = nextDirection.NextDirection; 
        Dir direction = chooseWay(xStart, yStart);

        setRoomConnectionsPath(lastDirection, direction, xStart, yStart);

        switch (direction) {
            case Dir.Up:
                yStart++;
                break;
            case Dir.Right:
                xStart++;
                break;
            case Dir.Down:
                yStart--;
                break;
            case Dir.Left:
                xStart--;
                break;
            case Dir.Count:
                //Nothing...
                break;
            default:

                break;
        }

        if (checkIfPathFree(xStart, yStart) == false) {
            alwaysTrue = false;
            return nextDirection;
        }
        nextDir nextPath = new nextDir(xStart, yStart, direction);

        setRoomType("corridor", nextPath.XNext, nextPath.YNext);

        calculateEmptyness();

        //print ("x: " + nextRoom [0] + " y: " + nextRoom [1] + " emptyness: " + roomTiles [nextRoom [0], nextRoom [1]].Emptyness);

        return nextPath;
    }

    bool checkIfPathFree(int xCheck, int yCheck)
    {
        if (xCheck >= xDimension || yCheck >= yDimension || xCheck < 0 || yCheck < 0)
            return false;
        if (roomTiles[xCheck, yCheck].Type == "empty") {
            return true;
        } else {
            return false;
        }
    }

    Dir chooseWay(int xCheck, int yCheck)
    {
        List<Dir> possibleWays = new List<Dir>();
        double bestEmptyness = 0;
        Dir bestDirection = Dir.Count;

        for (Dir actDir = Dir.Up; actDir < Dir.Count; actDir++) {

            switch (actDir) {

                case Dir.Up:
                    if (checkIfGoodPath(xCheck, yCheck + 1, actDir)) {
                        if (roomTiles[xCheck, yCheck + 1].Emptyness > bestEmptyness) {
                            bestEmptyness = roomTiles[xCheck, yCheck + 1].Emptyness;
                            bestDirection = actDir;
                        }
                        possibleWays.Add(actDir);
                    }
                    break;

                case Dir.Right:

                    if (checkIfGoodPath(xCheck + 1, yCheck, actDir)) {
                        if (roomTiles[xCheck + 1, yCheck].Emptyness > bestEmptyness) {
                            bestEmptyness = roomTiles[xCheck + 1, yCheck].Emptyness;
                            bestDirection = actDir;
                        }
                        possibleWays.Add(actDir);
                    }
                    break;

                case Dir.Down:

                    if (checkIfGoodPath(xCheck, yCheck - 1, actDir)) {
                        if (roomTiles[xCheck, yCheck - 1].Emptyness > bestEmptyness) {
                            bestEmptyness = roomTiles[xCheck, yCheck - 1].Emptyness;
                            bestDirection = actDir;
                        }
                        possibleWays.Add(actDir);
                    }
                    break;

                case Dir.Left:

                    if (checkIfGoodPath(xCheck - 1, yCheck, actDir)) {
                        if (roomTiles[xCheck - 1, yCheck].Emptyness > bestEmptyness) {
                            bestEmptyness = roomTiles[xCheck - 1, yCheck].Emptyness;
                            bestDirection = actDir;
                        }
                        possibleWays.Add(actDir);
                    }
                    break;

                default:
                    break;
            }
        }
        if (possibleWays.Count < 1)
            return Dir.Count;
        if (UnityEngine.Random.Range(0, 8) == 4)
            return possibleWays[UnityEngine.Random.Range(0, possibleWays.Count)];
        else
            return bestDirection;
    }

    bool checkIfGoodPath(int xCheck, int yCheck, Dir direction)
    {
        switch (direction) {
            case Dir.Up:
                if (checkIfPathFree(xCheck, yCheck + 1) && checkIfPathFree(xCheck + 1, yCheck) &&
                    checkIfPathFree(xCheck - 1, yCheck) && checkIfPathFree(xCheck, yCheck))
                    return true;
                break;
            case Dir.Right:
                if (checkIfPathFree(xCheck + 1, yCheck) && checkIfPathFree(xCheck, yCheck + 1) &&
                    checkIfPathFree(xCheck, yCheck - 1) && checkIfPathFree(xCheck, yCheck))
                    return true;
                break;
            case Dir.Down:
                if (checkIfPathFree(xCheck, yCheck - 1) && checkIfPathFree(xCheck + 1, yCheck) &&
                    checkIfPathFree(xCheck - 1, yCheck) && checkIfPathFree(xCheck, yCheck))
                    return true;
                break;
            case Dir.Left:
                if (checkIfPathFree(xCheck - 1, yCheck) && checkIfPathFree(xCheck, yCheck + 1) &&
                    checkIfPathFree(xCheck, yCheck - 1) && checkIfPathFree(xCheck, yCheck))
                    return true;
                break;
            default:
                return false;
        }
        return false;
    }

    void calculateEmptyness()
    {
        double actEmptyness = 0.0;
        int relevantRooms = 26; //1: 8; 2: 26; 3: 50
        int range = 2;

        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                actEmptyness = 0.0;
                for (int xLocal = x - range; xLocal <= x + range; xLocal++) {
                    for (int yLocal = y - range; yLocal <= y + range; yLocal++) {
                        if (xLocal < 0 || xLocal >= xDimension || yLocal < 0 || yLocal >= yDimension || (xLocal == x && yLocal == y)) {
                            actEmptyness = actEmptyness + 0;
                        } else {
                            if (roomTiles[xLocal, yLocal].Type == "empty")
                                actEmptyness += 100.0 / relevantRooms;
                        }
                    }
                }
                roomTiles[x, y].Emptyness = actEmptyness;

            }
        }
    }

    void loadMap(string fileName, string fileEnding)
    {
        string path = @"C:\Users\Oliver\Desktop\" + fileName + fileEnding;

        string readText = File.ReadAllText(path);

        int s = 0;
        for (int y = yDimension - 1; y >= 0; y--) {
            for (int x = 0; x < xDimension; x++) {

                Room roomTypeFound = roomTypes.Find(z => z.Symbol == readText[s]);
                setRoomType(roomTypeFound.Type, x, y);
                s++;
            }
            s += 2;
        }
    }

    void saveMap(string fileName, string fileEnding)
    {
        string path = @"C:\Users\Oliver\Desktop\" + fileName + fileEnding;
        //if (!File.Exists (path)) {
        string createText = Environment.NewLine;
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {
                createText += roomTiles[x, y].Symbol;
            }
            createText += Environment.NewLine;
        }
        File.WriteAllText(path, createText);
        print("Printed");
    }

    static RTP[,] RotateMatrix(RTP[,] matrix)
    {
        RTP[,] ret = new RTP[3, 3];
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    ret[i, j] = matrix[3 - j - 1, i];
                }
            }
        return ret;
    }
    void printRoomTileParts(RTP[,] toPrint)
    {
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                print(toPrint[i,j] + " ");
            }
        }
    }
    void visualizeMap()
    {
        //destroyAll();
        
        for (int x = 0; x < xDimension; x++) {
            for (int y = 0; y < yDimension; y++) {

                for (int i = 0; i < 3; ++i){
                    for (int j = 0; j < 3; ++j){
                 
                        actTile = new RoomTile(GameObject.CreatePrimitive(PrimitiveType.Cube));
                        actTile.TileObject.transform.position = new Vector3(0.125f/3 + x / 4.0f + i*0.25f / 3, 0.125f/3 + y / 4.0f + j*0.25f / 3, -5.0f);
                        actTile.TileObject.transform.localScale = new Vector3(0.25f / 3, 0.25f / 3, 0.25f / 3);//(0.1f, 0.1f, 0.1f); //0.25f/3
                        Material newMaterial = new Material(Shader.Find("Standard"));
                        switch (roomTiles[x,y].RoomTileParts[i, j])
                        {
                            case RTP.Wall:

                                newMaterial.color = new Color(0, 0, 0);
                                actTile.TileObject.GetComponent<MeshRenderer>().material = newMaterial;
                                break;
                            case RTP.Room:

                                newMaterial.color = new Color(255, 100, 0);
                                actTile.TileObject.GetComponent<MeshRenderer>().material = newMaterial;
                                print("Room");
                                break;
                            case RTP.Corridor:

                                newMaterial.color = new Color(200, 255, 0);
                                actTile.TileObject.GetComponent<MeshRenderer>().material = newMaterial;
                                print("corridor");
                                break;
                            default:
                                print("nicht erkannt");
                                break;
                        }
                  

                    }
                }
            }
        }
    }
    
}



	



