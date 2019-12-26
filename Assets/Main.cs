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
	void initializeRoomTypes ()
	{
		bool[] emptyRoom = new bool[]{ true, true, true, true };
		roomTypes.Add (new Room ("empty", emptyRoom, new Color (255, 0, 0), 100.0, ' '));
		roomTypes.Add (new Room ("entrance", emptyRoom, new Color (255, 0, 255), 100.0, 'x'));
		roomTypes.Add (new Room ("corridor", emptyRoom, new Color (0, 200, 255), 100.0, 'o'));
		roomTypes.Add (new Room ("room1", emptyRoom, new Color (0, 0, 255), 100.0, '◙'));
		roomTypes.Add (new Room ("boss", emptyRoom, new Color (2, 100, 5), 'b'));
	}
	//#####################################################################################################
	// Variables
	//#####################################################################################################
	const int xDimension = 40, yDimension = 40;
	RoomTile[,] roomTiles;
	RoomTile actTile;
	List<Room> roomTypes = new List<Room> ();
	List<roomDimension> createdRooms = new List<roomDimension> ();
	int[] nextWay;
	int xEntrance = 0, yEntrance = 0;
	//#####################################################################################################
	// Initialization
	//#####################################################################################################
	void Start ()
	{

		transform.position = new Vector3 (xDimension / 8.0f, yDimension / 8.0f, -(xDimension + xDimension));
		roomTiles = new RoomTile[xDimension, yDimension];

		initializeRoomTypes ();
		initializeRooms ();
		nextWay = new int[3];

		xEntrance = UnityEngine.Random.Range (0, xDimension);
		yEntrance = UnityEngine.Random.Range (0, yDimension);

		setRoomType ("entrance", xEntrance, yEntrance);
		setEntrance (xEntrance, yEntrance);

		print ("x: " + xEntrance + " y: " + yEntrance + " emptyness: " + roomTiles [nextWay [0], nextWay [1]].Emptyness);

		nextWay = simplePath (xEntrance, yEntrance);

		nextWay = simpleRoom ();
		calculateEmptyness ();

		print ("x: " + nextWay [0] + " y: " + nextWay [1] + " emptyness: " + roomTiles [nextWay [0], nextWay [1]].Emptyness);
		nextWay = simplePath (nextWay [0], nextWay [1]);
		setRoomType ("corridor", nextWay [0], nextWay [1]);
		calculateEmptyness ();
	}

	//#####################################################################################################
	// Update is called once per frame
	//#####################################################################################################
	void Update ()
	{
//
//		nextWay = simplePath (nextWay [0], nextWay [1]);
//		setRoomType ("corridor", nextWay [0], nextWay [1]);
//		calculateEmptyness ();
//		print ("x: " + nextWay [0] + " y: " + nextWay [1] + " emptyness: " + roomTiles [nextWay [0], nextWay [1]].Emptyness);


		if (Input.GetMouseButtonDown (1))
			saveMap ("Map", ".txt");

		if (Input.GetMouseButtonDown (0)) {
			destroyAll ();
			Start ();
		}
	}

	//#####################################################################################################
	void initializeRooms ()
	{
		for (int x = 0; x < xDimension; x++) {
			for (int y = 0; y < yDimension; y++) {
				actTile = new RoomTile (GameObject.CreatePrimitive (PrimitiveType.Cube));
				actTile.TileObject.transform.position = new Vector3 (0.125f + x / 4.0f, 0.125f + y / 4.0f, 0);
				actTile.TileObject.transform.localScale = new Vector3 (0.25f, 0.25f, 0.25f);
				roomTiles [x, y] = actTile;

			}
		}
	}
	//#####################################################################################################
	void setRoomType (string roomType, int xPos, int yPos)
	{
		
		RoomTile roomToChange = roomTiles [xPos, yPos];

		Room roomTypeFound = roomTypes.Find (x => x.Type == roomType);

		overwriteRoom (roomTypeFound, roomToChange);

		setRoomColor (roomToChange);

	}
	//#####################################################################################################
	void destroyAll ()
	{
		
		foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>())
			if (o.tag != "MainCamera")
				Destroy (o);
	}

	void setEntrance (int xPos, int yPos)
	{
		RoomTile roomToChange = roomTiles [xPos, yPos];
		roomToChange.Connections [0] = false;
		roomToChange.Connections [1] = false;
		roomToChange.Connections [2] = false;
		roomToChange.Connections [3] = false;
		roomToChange.Connections [UnityEngine.Random.Range (0, 3)] = true;
	}

	void overwriteRoom (Room roomTypeFound, RoomTile roomToChange)
	{
		roomToChange.Type = roomTypeFound.Type;
		roomToChange.Color = roomTypeFound.Color;
		roomToChange.Symbol = roomTypeFound.Symbol;
	}

	void setRoomColor (RoomTile roomToChange)
	{
		Material newMaterial = new Material (Shader.Find ("Standard"));
		newMaterial.color = roomToChange.Color;

		roomToChange.TileObject.GetComponent<MeshRenderer> ().material = newMaterial;
	}

	int[] simpleRoom ()
	{
		roomDimension rDim = spaceforRoom (UnityEngine.Random.Range (3, 9), UnityEngine.Random.Range (3, 9)); //ATM nur gerade
		//print ("xmin: " + roomDim [1] + " ymin: " + roomDim [2] + " xmax: " + roomDim [3] + " ymax: " + roomDim [4]);

		int[] roomExit = new int[3];
		if (rDim.RoomPossible == true) {
			for (int x = rDim.XRoomCornerMin; x <= rDim.XRoomCornerMax; x++) {
				for (int y = rDim.YRoomCornerMin; y <= rDim.YRoomCornerMax; y++) {
					setRoomType ("room1", x, y);
				}
			}
		}
		//return next Coordinates for corridor
		roomExit [0] = rDim.XExit;
		roomExit [1] = rDim.YExit;
		roomExit [2] = nextWay [2];

		return roomExit;
	}

	roomDimension spaceforRoom (int xRoomSize, int yRoomSize)
	{
		roomDimension rDim = new roomDimension ();
		int xStart = nextWay [0], yStart = nextWay [1], direction = nextWay [2];

		switch (direction) {
		case 0:
			rDim.XRoomCornerMin = xStart - ((xRoomSize - 1) / 2);
			rDim.XRoomCornerMax = xStart + ((xRoomSize - 1) / 2);
			rDim.YRoomCornerMin = yStart + 1;
			rDim.YRoomCornerMax = yStart + yRoomSize;
			rDim.XExit = xStart;
			rDim.YExit = rDim.YRoomCornerMax + 1;
			break;
		case 1:
			rDim.XRoomCornerMin = xStart + 1;
			rDim.YRoomCornerMin = yStart - ((yRoomSize - 1) / 2);
			rDim.XRoomCornerMax = xStart + xRoomSize;
			rDim.YRoomCornerMax = yStart + ((yRoomSize - 1) / 2);
			rDim.XExit = rDim.XRoomCornerMax + 1;
			rDim.YExit = yStart;
			break;
		case 2:
			rDim.YRoomCornerMin = yStart - yRoomSize;
			rDim.XRoomCornerMax = xStart + ((xRoomSize - 1) / 2);
			rDim.YRoomCornerMax = yStart - 1;
			rDim.XRoomCornerMin = xStart - ((xRoomSize - 1) / 2);
			rDim.XExit = xStart;
			rDim.YExit = rDim.YRoomCornerMin - 1;
			break;
		case 3:
			rDim.XRoomCornerMin = xStart - xRoomSize;			
			rDim.YRoomCornerMin = yStart - ((yRoomSize - 1) / 2);
			rDim.XRoomCornerMax = xStart - 1;
			rDim.YRoomCornerMax = yStart + ((yRoomSize - 1) / 2);
			rDim.XExit = rDim.XRoomCornerMin - 1;
			rDim.YExit = yStart;
			break;
		default:
			break;
		}
		if (rDim.XRoomCornerMax > xDimension || rDim.YRoomCornerMax > yDimension || rDim.XRoomCornerMin < 0 || rDim.YRoomCornerMin < 0
		    || rDim.XExit > xDimension || rDim.YExit > yDimension || rDim.XExit < 0 || rDim.YExit < 0) {
			rDim.RoomPossible = false;
			return rDim;
		}
		for (int x = rDim.XRoomCornerMin; x <= rDim.XRoomCornerMax; x++) {
			for (int y = rDim.YRoomCornerMin; y <= rDim.YRoomCornerMax; y++) {
				if (roomTiles [x, y].Type != "empty" && x != xStart && y != yStart)
					rDim.RoomPossible = false;
			}
		}

		return rDim;
	}

	int[] simplePath ()
	{
		int xNext, xStart, yNext, yStart, nextDirection;
		xNext = xStart = nextWay [0];
		yNext = yStart = nextWay [1];

		RoomTile actRoom = roomTiles [xStart, yStart];

		nextDirection = chooseWay (actRoom.Connections, xStart, yStart);
		switch (nextDirection) {
		case 0:
			yNext++;
			roomTiles [xStart, yStart].Connections [0] = false;
			roomTiles [xNext, yNext].Connections [2] = false;
			break;
		case 1:
			xNext++;
			roomTiles [xStart, yStart].Connections [1] = false;
			roomTiles [xNext, yNext].Connections [3] = false;
			break;
		case 2:
			yNext--;
			roomTiles [xStart, yStart].Connections [2] = false;
			roomTiles [xNext, yNext].Connections [0] = false;
			break;
		case 3:
			xNext--;
			roomTiles [xStart, yStart].Connections [3] = false;
			roomTiles [xNext, yNext].Connections [1] = false;
			break;

		default:
			
			break;
		}

		int[] nextRoom = new int[3];
		nextRoom [0] = xNext;
		nextRoom [1] = yNext;
		nextRoom [2] = nextDirection;

		setRoomType ("corridor", nextRoom [0], nextRoom [1]);

		calculateEmptyness ();

		print ("x: " + nextRoom [0] + " y: " + nextRoom [1] + " emptyness: " + roomTiles [nextRoom [0], nextRoom [1]].Emptyness);

		return nextRoom;
	}

	bool checkIfPathFree (int xCheck, int yCheck)
	{
		if (roomTiles [xCheck, yCheck].Type == "empty") {
			return true;
		} else {
			return false;
		}
	}

	int chooseWay (int xCheck, int yCheck)
	{
		List<int> possibleWays = new List<int> ();
		double bestEmptyness = 0;
		int bestDirection = 1000;
		for (int x = 0; x < 4; x++) {
			switch (x) {
			case 0:
				if (yCheck + 1 < yDimension) {
					if (checkIfPathFree (xCheck, yCheck + 1)) {

						if (roomTiles [xCheck, yCheck + 1].Emptyness > bestEmptyness) {
							bestEmptyness = roomTiles [xCheck, yCheck + 1].Emptyness;
							bestDirection = x;
						}
						//for (int c = 0; c < roomTiles [xCheck, yCheck + 1].Emptyness; c++)
						possibleWays.Add (x);
					}
					
				}
				break;
			case 1:
				if (xCheck + 1 < xDimension) {
					if (checkIfPathFree (xCheck + 1, yCheck)) {
						if (roomTiles [xCheck + 1, yCheck].Emptyness > bestEmptyness) {
							bestEmptyness = roomTiles [xCheck + 1, yCheck].Emptyness;
							bestDirection = x;
						}
						//for (int c = 0; c < roomTiles [xCheck + 1, yCheck].Emptyness; c++)
						possibleWays.Add (x);
					}
				}
				break;
			case 2:
				if (yCheck - 1 >= 0) {
					if (checkIfPathFree (xCheck, yCheck - 1)) {

						if (roomTiles [xCheck, yCheck - 1].Emptyness > bestEmptyness) {
							bestEmptyness = roomTiles [xCheck, yCheck - 1].Emptyness;
							bestDirection = x;
						}
						///for (int c = 0; c < roomTiles [xCheck, yCheck - 1].Emptyness; c++)
						possibleWays.Add (x);
					}
				}
				break;
			case 3:
				if (xCheck - 1 >= 0) {
					if (checkIfPathFree (xCheck - 1, yCheck)) {

						if (roomTiles [xCheck - 1, yCheck].Emptyness > bestEmptyness) {
							bestEmptyness = roomTiles [xCheck - 1, yCheck].Emptyness;
							bestDirection = x;
						}
						//for (int c = 0; c < roomTiles [xCheck - 1, yCheck].Emptyness; c++)
						possibleWays.Add (x);
					}
				}
				break;
			default:
				break;
			}
		}
		if (UnityEngine.Random.Range (0, 2) == 0)
			return possibleWays [UnityEngine.Random.Range (0, possibleWays.Count)];
		else
			return (int)bestDirection;
	}

	void calculateEmptyness ()
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
							if (roomTiles [xLocal, yLocal].Type == "empty")
								actEmptyness += 100.0 / relevantRooms;
						}
					}
				}
				roomTiles [x, y].Emptyness = actEmptyness;

			}
		}
	}

	void saveMap (string fileName, string fileEnding)
	{
		string path = @"C:\Users\Oliver Laptop\Desktop\" + fileName + fileEnding;
		//if (!File.Exists (path)) {
		string createText = Environment.NewLine;
		for (int x = 0; x < xDimension; x++) {
			for (int y = 0; y < yDimension; y++) {
				createText += roomTiles [x, y].Symbol;
			}
			createText += Environment.NewLine;
		}
		File.WriteAllText (path, createText);
		print ("Printed");
	}





}
	



