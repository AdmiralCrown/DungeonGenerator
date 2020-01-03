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
        private ConnectionType upCon;
        private ConnectionType rightCon;
        private ConnectionType downCon;
        private ConnectionType leftCon;
        private RTP[,] roomTileParts;

        public Room (string type, Color color, double emptyness, char symbol)
		{
			this.type = type;
			this.color = color;
			this.emptyness = emptyness;
			this.symbol = symbol;
            this.upCon = ConnectionType.None;
            this.downCon = ConnectionType.None;
            this.leftCon = ConnectionType.None;
            this.rightCon = ConnectionType.None;
            this.roomTileParts = new RTP[3, 3] { { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Wall, RTP.Wall, RTP.Wall } };
        }

		public Room (string type, Color color, double emptyness)
		{
			this.type = type;
			this.color = color;
			this.emptyness = emptyness;
			this.symbol = ' ';
            this.upCon = ConnectionType.None;
            this.downCon = ConnectionType.None;
            this.leftCon = ConnectionType.None;
            this.rightCon = ConnectionType.None;
            this.roomTileParts = new RTP[3, 3] { { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Wall, RTP.Wall, RTP.Wall } };
        }

		public Room ()
		{
			this.type = "empty";
			this.color = new Color (0, 0, 0);
			this.emptyness = 100.0;
			this.symbol = ' ';
            this.upCon = ConnectionType.None;
            this.downCon = ConnectionType.None;
            this.leftCon = ConnectionType.None;
            this.rightCon = ConnectionType.None;
            this.roomTileParts = new RTP[3, 3] { { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Wall, RTP.Wall, RTP.Wall }, { RTP.Wall, RTP.Wall, RTP.Wall } };
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

        internal ConnectionType UpCon
        {
            get
            {
                return upCon;
            }

            set
            {
                upCon = value;
            }
        }

        internal ConnectionType RightCon
        {
            get
            {
                return rightCon;
            }

            set
            {
                rightCon = value;
            }
        }

        internal ConnectionType DownCon
        {
            get
            {
                return downCon;
            }

            set
            {
                downCon = value;
            }
        }

        internal ConnectionType LeftCon
        {
            get
            {
                return leftCon;
            }

            set
            {
                leftCon = value;
            }
        }

        internal RTP[,] RoomTileParts
        {
            get
            {
                return roomTileParts;
            }

            set
            {
                roomTileParts = value;
            }
        }
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

	enum OpMode
	{
		ClickOnly,
		AutomaticMode,
		LoadMap,
	};

    enum ConnectionType
    {
        None,
        Corridor,
        Room,
    };

    enum Dir
    {
        Up,
        Right,
        Down,
        Left,
        Count,
    };

    enum RTP
    {
        Wall,
        Corridor,
        Room,
    }
    class nextDir
    {
        private int xNext;
        private int yNext;
        private Dir nextDirection;

        public nextDir()
        {
            this.xNext = 0;
            this.yNext = 0;
            this.nextDirection = Dir.Count;
        }

        public nextDir(int xNext, int yNext, Dir nextDirection)
        {
            this.xNext = xNext;
            this.yNext = yNext;
            this.nextDirection = nextDirection;
        }
 
        public int XNext
        {
            get { return xNext; }
            set { xNext = value; }
        }
        public int YNext
        {
            get { return yNext; }
            set { yNext = value; }
        }
        public Dir NextDirection
        {
            get { return nextDirection; }
            set { nextDirection = value; }
        }
    }
}

