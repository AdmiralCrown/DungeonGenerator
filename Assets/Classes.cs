using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DungeonGeneratorClasses;
using System.IO;
using System.Text;
using System;

namespace DungeonGeneratorClasses
{
	// Classes
	//#####################################################################################################
	public class Room
	{
		//Up 0, Right 1, Down 2, Left 3
		private string type;
		private Color color;
		private double emptyness;
		private char symbol;

		#region constructor_getter_setter

		public Room (string type, Color color, double emptyness, char symbol)
		{
			this.type = type;
			this.color = color;
			this.emptyness = emptyness;
			this.symbol = symbol;
		}

		public Room (string type, Color color, double emptyness)
		{
			this.type = type;
			this.color = color;
			this.emptyness = emptyness;
			this.symbol = ' ';
		}

		public Room ()
		{
			this.type = "empty";
			this.color = new Color (0, 0, 0);
			this.emptyness = 100.0;
			this.symbol = ' ';
		}

		public string Type {
			get { return type; }
			set { type = value; }
		}

		public Color Color {
			get { return color; }
			set { color = value; }
		}

		public double Emptyness {
			get { return emptyness; }
			set { emptyness = value; }
		}

		public char Symbol {
			get { return symbol; }
			set { symbol = value; }
		}

		#endregion
	}

	public class RoomTile : Room
	{
		private GameObject tileObject;

		public RoomTile (GameObject additional) : base ()
		{
			this.tileObject = additional;
		}

		public GameObject TileObject {
			get { return tileObject; }
			set { tileObject = value; }
		}

	}

	public class Data
	{


	}

	public class roomDimension
	{
		private int xRoomCornerMin;
		private int yRoomCornerMin;
		private int xRoomCornerMax;
		private int yRoomCornerMax;
		private bool roomPossible;
		private int xExit;
		private int yExit;

		public roomDimension (int xMin, int xMax, int yMin, int yMax, bool roomPossible)
		{
			this.xRoomCornerMin = xMin;
			this.xRoomCornerMax = xMax;
			this.yRoomCornerMin = yMin;
			this.yRoomCornerMax = yMax;
			this.roomPossible = roomPossible;
		}

		public roomDimension ()
		{
			this.xRoomCornerMin = 0;
			this.xRoomCornerMax = 0;
			this.yRoomCornerMin = 0;
			this.yRoomCornerMax = 0;
			this.roomPossible = true;
			this.xExit = 0;
			this.yExit = 0;
		}

		public int XRoomCornerMin {
			get { return xRoomCornerMin; }
			set { xRoomCornerMin = value; }
		}

		public int YRoomCornerMin {
			get { return yRoomCornerMin; }
			set { yRoomCornerMin = value; }
		}

		public int XRoomCornerMax {
			get { return xRoomCornerMax; }
			set { xRoomCornerMax = value; }
		}

		public int YRoomCornerMax {
			get { return yRoomCornerMax; }
			set { yRoomCornerMax = value; }
		}

		public bool RoomPossible {
			get { return roomPossible; }
			set { roomPossible = value; }
		}

		public int XExit {
			get { return xExit; }
			set { xExit = value; }
		}

		public int YExit {
			get { return yExit; }
			set { yExit = value; }
		}

	}
}

