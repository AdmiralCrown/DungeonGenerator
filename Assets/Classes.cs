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
        private Dir directionExit;
        private int xEntrance;
        private int yEntrance;
        private Dir directionEntrance;


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
            this.xEntrance = 0;
            this.yEntrance = 0;
            directionExit = Dir.Count;
            directionEntrance = Dir.Count;
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

        public int XEntrance
        {
            get
            {
                return xEntrance;
            }

            set
            {
                xEntrance = value;
            }
        }

        public int YEntrance
        {
            get
            {
                return yEntrance;
            }

            set
            {
                yEntrance = value;
            }
        }

        internal Dir DirectionEntrance
        {
            get
            {
                return directionEntrance;
            }

            set
            {
                directionEntrance = value;
            }
        }

        internal Dir DirectionExit
        {
            get
            {
                return directionExit;
            }

            set
            {
                directionExit = value;
            }
        }
    }

    public class newRTP
    {
        private RTP[,] parts;

        public newRTP()
        {
            parts = new RTP[3,3];
        }
        public newRTP(int[] row1, int[] row2, int[] row3)
        {
            parts = new RTP[3,3];
            parts[0, 0] = (RTP)row1[0];
            parts[0, 1] = (RTP)row1[1];
            parts[0, 2] = (RTP)row1[2];
            parts[1, 0] = (RTP)row2[0];
            parts[1, 1] = (RTP)row2[1];
            parts[1, 2] = (RTP)row2[2];
            parts[2, 0] = (RTP)row3[0];
            parts[2, 1] = (RTP)row3[1];
            parts[2, 2] = (RTP)row3[2];
        }

        internal RTP[,] Parts
        {
            get
            {
                return parts;
            }

            set
            {
                parts = value;
            }
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
        Room,
        Corridor,        
    }
    class Position
    {
        private int x;
        private int y;

        public Position()
        {
            x = -1;
            y = -1;
        }

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }
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

