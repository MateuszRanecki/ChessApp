using System.Collections.Generic;

namespace Chess_App
{
    internal struct PieceMovesList
    {
        internal readonly List<byte> Moves;

        internal PieceMovesList(List<byte> moves)
        {
            Moves = moves;
        }
    }

    internal struct MoveArrays
    {
        internal static PieceMovesList[] BishopMoves1;
        internal static byte[] BishopTotalMoves1;
        
        internal static PieceMovesList[] BishopMoves2;
        internal static byte[] BishopTotalMoves2;

        internal static PieceMovesList[] BishopMoves3;
        internal static byte[] BishopTotalMoves3;

        internal static PieceMovesList[] BishopMoves4;
        internal static byte[] BishopTotalMoves4;

        internal static PieceMovesList[] BlackPawnMoves;
        internal static byte[] MovesListBlackPawn;

        internal static PieceMovesList[] WhitePawnMoves;
        internal static byte[] MovesListWhitePawn;

        internal static PieceMovesList[] KnightMoves;
        internal static byte[] KnightTotalMoves;

        internal static PieceMovesList[] QueenMoves1;
        internal static byte[] QueenTotalMoves1;
        internal static PieceMovesList[] QueenMoves2;
        internal static byte[] QueenTotalMoves2;
        internal static PieceMovesList[] QueenMoves3;
        internal static byte[] QueenTotalMoves3;
        internal static PieceMovesList[] QueenMoves4;
        internal static byte[] QueenTotalMoves4;
        internal static PieceMovesList[] QueenMoves5;
        internal static byte[] QueenTotalMoves5;
        internal static PieceMovesList[] QueenMoves6;
        internal static byte[] QueenTotalMoves6;
        internal static PieceMovesList[] QueenMoves7;
        internal static byte[] QueenTotalMoves7;
        internal static PieceMovesList[] QueenMoves8;
        internal static byte[] QueenTotalMoves8;

        internal static PieceMovesList[] RookMoves1;
        internal static byte[] RookTotalMoves1;
        internal static PieceMovesList[] RookMoves2;
        internal static byte[] RookTotalMoves2;
        internal static PieceMovesList[] RookMoves3;
        internal static byte[] RookTotalMoves3;
        internal static PieceMovesList[] RookMoves4;
        internal static byte[] RookTotalMoves4;

        internal static PieceMovesList[] KingMoves;
        internal static byte[] KingTotalMoves;
    }

    internal static class ChessPieceMoves
    {
        internal static bool Initiated;

        private static byte Position(byte col, byte row)
        {
            return (byte)(col + (row * 8));
        }

        #region IntitiateMotionMethods

        internal static void InitiateChessPieceMotion()
        {
            if (Initiated)
            {
                return;
            }

            Initiated = true;

            MoveArrays.WhitePawnMoves = new PieceMovesList[64];
            MoveArrays.MovesListWhitePawn = new byte[64];

            MoveArrays.BlackPawnMoves = new PieceMovesList[64];
            MoveArrays.MovesListBlackPawn = new byte[64];

            MoveArrays.KnightMoves = new PieceMovesList[64];
            MoveArrays.KnightTotalMoves = new byte[64];

            MoveArrays.BishopMoves1 = new PieceMovesList[64];
            MoveArrays.BishopTotalMoves1 = new byte[64];

            MoveArrays.BishopMoves2 = new PieceMovesList[64];
            MoveArrays.BishopTotalMoves2 = new byte[64];

            MoveArrays.BishopMoves3 = new PieceMovesList[64];
            MoveArrays.BishopTotalMoves3 = new byte[64];

            MoveArrays.BishopMoves4 = new PieceMovesList[64];
            MoveArrays.BishopTotalMoves4 = new byte[64];

            MoveArrays.RookMoves1 = new PieceMovesList[64];
            MoveArrays.RookTotalMoves1 = new byte[64];

            MoveArrays.RookMoves2 = new PieceMovesList[64];
            MoveArrays.RookTotalMoves2 = new byte[64];

            MoveArrays.RookMoves3 = new PieceMovesList[64];
            MoveArrays.RookTotalMoves3 = new byte[64];

            MoveArrays.RookMoves4 = new PieceMovesList[64];
            MoveArrays.RookTotalMoves4 = new byte[64];

            MoveArrays.QueenMoves1 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves1 = new byte[64];

            MoveArrays.QueenMoves2 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves2 = new byte[64];

            MoveArrays.QueenMoves3 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves3 = new byte[64];

            MoveArrays.QueenMoves4 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves4 = new byte[64];

            MoveArrays.QueenMoves5 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves5 = new byte[64];

            MoveArrays.QueenMoves6 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves6 = new byte[64];

            MoveArrays.QueenMoves7 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves7 = new byte[64];

            MoveArrays.QueenMoves8 = new PieceMovesList[64];
            MoveArrays.QueenTotalMoves8 = new byte[64];

            MoveArrays.KingMoves = new PieceMovesList[64];
            MoveArrays.KingTotalMoves = new byte[64];
            
            WhitePawnAllMoves();
            BlackPawnAllMoves();
            KnightAllMoves();
            BishopAllMoves();
            RookAllMoves();
            QueenAllMoves();
            KingAllMoves();
        }

        private static void BlackPawnAllMoves()
        {
            for (byte index = 8; index <= 55; index++)
            {
                var moveList = new PieceMovesList(new List<byte>());
                
                byte x = (byte)(index % 8);
                byte y = (byte)((index / 8));
                
                //Zbicie 
                if (y < 7 && x < 7)
                {
                    moveList.Moves.Add((byte)(index + 8 + 1));
                    MoveArrays.MovesListBlackPawn[index]++;
                }
                if (x > 0 && y < 7)
                {
                    moveList.Moves.Add((byte)(index + 8 - 1));
                    MoveArrays.MovesListBlackPawn[index]++;
                }
                
                //Ruch do przodu
                moveList.Moves.Add((byte)(index + 8));
                MoveArrays.MovesListBlackPawn[index]++;

                //Ruch o 2 pola
                if (y == 1)
                {
                    moveList.Moves.Add((byte)(index + 16));
                    MoveArrays.MovesListBlackPawn[index]++;
                }

                MoveArrays.BlackPawnMoves[index] = moveList;
            }
        }

        private static void WhitePawnAllMoves()
        {
            for (byte index = 8; index <= 55; index++)
            {
                byte x = (byte)(index % 8);
                byte y = (byte)((index / 8));

                var moveList = new PieceMovesList(new List<byte>());

                //zbicie
                if (x < 7 && y > 0)
                {
                    moveList.Moves.Add((byte)(index - 8 + 1));
                    MoveArrays.MovesListWhitePawn[index]++;
                }
                if (x > 0 && y > 0)
                {
                    moveList.Moves.Add((byte)(index - 8 - 1));
                    MoveArrays.MovesListWhitePawn[index]++;
                }

                //ruch do przodu
                moveList.Moves.Add((byte)(index - 8));
                MoveArrays.MovesListWhitePawn[index]++;

                //ruch o 2 pola
                if (y == 6)
                {
                    moveList.Moves.Add((byte)(index - 16));
                    MoveArrays.MovesListWhitePawn[index]++;
                }

                MoveArrays.WhitePawnMoves[index] = moveList;
            }
        }

        private static void KnightAllMoves()
        {
            for (byte y = 0; y < 8; y++)
            {
                for (byte x = 0; x < 8; x++)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveList = new PieceMovesList(new List<byte>());
                    
                    byte move;

                    if (y < 6 && x > 0)
                    {
                        move = Position((byte)(y + 2), (byte)(x - 1));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 1 && x < 7)
                    {
                        move = Position((byte)(y - 2), (byte)(x + 1));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 1 && x > 0)
                    {
                        move = Position((byte)(y - 2), (byte)(x - 1));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y < 6 && x < 7)
                    {
                        move = Position((byte)(y + 2), (byte)(x + 1));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 0 && x < 6)
                    {
                        move = Position((byte)(y - 1), (byte)(x + 2));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y < 7 && x > 1)
                    {
                        move = Position((byte)(y + 1), (byte)(x - 2));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 0 && x > 1)
                    {
                        move = Position((byte)(y - 1), (byte)(x - 2));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }
                    
                    if (y < 7 && x < 6)
                    {
                        move = Position((byte)(y + 1), (byte)(x + 2));

                        if (move < 64)
                        {
                            moveList.Moves.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    MoveArrays.KnightMoves[index] = moveList;
                }
            }
        }

        private static void BishopAllMoves()
        {
            for (byte y = 0; y < 8; y++)
            {
                for (byte x = 0; x < 8; x++)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveList = new PieceMovesList(new List<byte>());
                    byte move;

                    byte row = x;
                    byte col = y;

                    while (row < 7 && col < 7)
                    {
                        row++;
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.BishopTotalMoves1[index]++;
                    }

                    MoveArrays.BishopMoves1[index] = moveList;
                    moveList = new PieceMovesList(new List<byte>());

                    row = x;
                    col = y;

                    while (row < 7 && col > 0)
                    {
                        row++;
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.BishopTotalMoves2[index]++;
                    }

                    MoveArrays.BishopMoves2[index] = moveList;
                    moveList = new PieceMovesList(new List<byte>());

                    row = x;
                    col = y;

                    while (row > 0 && col < 7)
                    {
                        row--;
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.BishopTotalMoves3[index]++;
                    }

                    MoveArrays.BishopMoves3[index] = moveList;
                    moveList = new PieceMovesList(new List<byte>());

                    row = x;
                    col = y;

                    while (row > 0 && col > 0)
                    {
                        row--;
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.BishopTotalMoves4[index]++;
                    }

                    MoveArrays.BishopMoves4[index] = moveList;
                }
            }
        }

        private static void RookAllMoves()
        {
            for (byte y = 0; y < 8; y++)
            {
                for (byte x = 0; x < 8; x++)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveList = new PieceMovesList(new List<byte>());
                    byte move;

                    byte row = x;
                    byte col = y;

                    while (row < 7)
                    {
                        row++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.RookTotalMoves1[index]++;
                    }

                    MoveArrays.RookMoves1[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (row > 0)
                    {
                        row--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.RookTotalMoves2[index]++;
                    }

                    MoveArrays.RookMoves2[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (col > 0)
                    {
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.RookTotalMoves3[index]++;
                    }

                    MoveArrays.RookMoves3[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (col < 7)
                    {
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.RookTotalMoves4[index]++;
                    }

                    MoveArrays.RookMoves4[index] = moveList;
                }
            }
        }

        private static void QueenAllMoves()
        {
            for (byte y = 0; y < 8; y++)
            {
                for (byte x = 0; x < 8; x++)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveList = new PieceMovesList(new List<byte>());
                    byte move;

                    byte row = x;
                    byte col = y;

                    while (row < 7)
                    {
                        row++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves1[index]++;
                    }

                    MoveArrays.QueenMoves1[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (row > 0)
                    {
                        row--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves2[index]++;
                    }

                    MoveArrays.QueenMoves2[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (col > 0)
                    {
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves3[index]++;
                    }

                    MoveArrays.QueenMoves3[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (col < 7)
                    {
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves4[index]++;
                    }

                    MoveArrays.QueenMoves4[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (row < 7 && col < 7)
                    {
                        row++;
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves5[index]++;
                    }

                    MoveArrays.QueenMoves5[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (row < 7 && col > 0)
                    {
                        row++;
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves6[index]++;
                    }

                    MoveArrays.QueenMoves6[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (row > 0 && col < 7)
                    {
                        row--;
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves7[index]++;
                    }

                    MoveArrays.QueenMoves7[index] = moveList;

                    moveList = new PieceMovesList(new List<byte>());
                    row = x;
                    col = y;

                    while (row > 0 && col > 0)
                    {
                        row--;
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.QueenTotalMoves8[index]++;
                    }

                    MoveArrays.QueenMoves8[index] = moveList;
                }
            }
        }

        private static void KingAllMoves()
        {
            for (byte y = 0; y < 8; y++)
            {
                for (byte x = 0; x < 8; x++)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveList = new PieceMovesList(new List<byte>());
                    byte move;

                    byte row = x;
                    byte col = y;

                    if (row < 7)
                    {
                        row++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row > 0)
                    {
                        row--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (col > 0)
                    {
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (col < 7)
                    {
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row < 7 && col < 7)
                    {
                        row++;
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row < 7 && col > 0)
                    {
                        row++;
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row > 0 && col < 7)
                    {
                        row--;
                        col++;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }


                    row = x;
                    col = y;

                    if (row > 0 && col > 0)
                    {
                        row--;
                        col--;

                        move = Position(col, row);
                        moveList.Moves.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    MoveArrays.KingMoves[index] = moveList;
                }
            }
        }

        #endregion
    }
}