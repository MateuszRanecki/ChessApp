using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Chess_App
{
    public sealed class ChessEngine
    {
        #region InternalMembers

        internal List<OpeningMove> CurrentGameBook;
        internal List<OpeningMove> UndoGameBook;
        
        #endregion

        #region PrivateMembers

        private ChessBoard ChessBoard;
        private ChessBoard PreviousChessBoard;
        private ChessBoard UndoChessBoard;
        
        private Stack<ChessMove> MoveHistory;
        private List<OpeningMove> OpeningBook;

        private string pvLine;

        #endregion

        #region PublicMembers

        public enum Difficulty
        {
            Easy,
            Medium,
            Hard,
            VeryHard
        }

        public enum TimeSettings
        {
            Moves40In5Minutes,
            Moves40In10Minutes,
            Moves40In20Minutes,
            Moves40In30Minutes,
            Moves40In40Minutes,
            Moves40In60Minutes,
            Moves40In90Minutes,
        }

        public ChessPieceType PromoteToPieceType = ChessPieceType.Queen;

        public VariablePiecesTaken PiecesTakenCount = new VariablePiecesTaken();

        //State Variables
        public ChessPieceColor HumanPlayer;
        public bool Thinking;
        public bool TrainingMode;

        //Stats
        public int NodesSearched;
        public int NodesQuiessence;
        public byte PlyDepthSearched;
        public byte PlyDepthReached;
        public byte RootMovesSearched;

        public TimeSettings GameTimeSettings;

        public string FEN
        {
            get { return ChessBoard.Fen(false, ChessBoard); }
        }

        public ChessMove LastMove
        {
            get { return ChessBoard.LastMove; }
        }

        public Difficulty GameDifficulty
        {
           	get 
			{
				if (PlyDepthSearched == 3)
				{
					return Difficulty.Easy;
				}
				else if (PlyDepthSearched == 5)
				{
					return Difficulty.Medium;
				}
				else if (PlyDepthSearched == 6)
				{
					return Difficulty.Hard;
				}
				else if (PlyDepthSearched == 7)
				{
					return Difficulty.VeryHard;
				}

				return Difficulty.Medium;
			
			}
			set
            {
                if (value == Difficulty.Easy)
                {
                    PlyDepthSearched = 3;
                    GameTimeSettings = TimeSettings.Moves40In10Minutes;
                }
                else if (value == Difficulty.Medium)
                {
                    PlyDepthSearched = 5;
                    GameTimeSettings = TimeSettings.Moves40In20Minutes;
                }
                else if (value == Difficulty.Hard)
                {
                    PlyDepthSearched = 6;
                    GameTimeSettings = TimeSettings.Moves40In60Minutes;
                }
                else if (value == Difficulty.VeryHard)
                {
                    PlyDepthSearched = 7;
                    GameTimeSettings = TimeSettings.Moves40In90Minutes;
                }
            }
        }

        public ChessPieceColor WhoseMove
        {
            get { return ChessBoard.WhoseMove; }
            set { ChessBoard.WhoseMove = value; }
        }

        public bool StaleMate
        {
            get { return ChessBoard.StaleMate; }
            set { ChessBoard.StaleMate = value; }
        }
        public bool RepeatedMove
        {
            get
            {
                if (ChessBoard.RepeatedMove >= 3)
                {
                    return true;
                }

                return false;
            }
        }

        public string PvLine
        {
            get { return pvLine;}
        }

        public bool FiftyMove
        {
             get
             {
                 if (ChessBoard.HalfMoveClock >= 100)
                 {
                     return true;
                 }

                 return false;
             }
        }

        public bool InsufficientMaterial
        {
            get
            {
                return ChessBoard.InsufficientMaterial;
            }
        }

        #endregion

        #region Constructors

        public ChessEngine()
        {
            InitiateEngine();
            InitiateBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public ChessEngine(string fen)
        {
            InitiateEngine();
            InitiateBoard(fen);
        }

        public void NewGame()
        {
            InitiateEngine();
            InitiateBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        public void InitiateBoard(string fen)
        {
            ChessBoard = new ChessBoard(fen);

            if (!String.IsNullOrEmpty(fen))
            {
                ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
                PositionEvaluation.EvaluateBoardScore(ChessBoard);
            }
        }

        private void InitiateEngine()
        {
            GameDifficulty = Difficulty.Medium;

            MoveHistory = new Stack<ChessMove>();
            HumanPlayer = ChessPieceColor.White;
            OpeningBook = new List<OpeningMove>();
            CurrentGameBook = new List<OpeningMove>();
            ChessPieceMoves.InitiateChessPieceMotion();
            LoadOpeningBook();
        }

        #endregion

        #region Methods

        public void SetChessPieceSelection(byte boardColumn, byte boardRow,
                                          bool selection)
        {
            byte index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece == null)
            {
                return;
            }
            if (ChessBoard.Squares[index].Piece.PieceColor != HumanPlayer)
            {
                return;
            }
            if (ChessBoard.Squares[index].Piece.PieceColor != WhoseMove)
            {
                return;
            }
            ChessBoard.Squares[index].Piece.Selected = selection;
        }

        public int ValidateOpeningBook()
        {
            return VariableOpeningBook.ValidateOpeningBook(OpeningBook);
        }

        private static bool CheckForMate(ChessPieceColor whosTurn, ref ChessBoard chessBoard)
        {
            PositionSearch.SearchForMate(whosTurn, chessBoard, ref chessBoard.BlackMate,
                                 ref chessBoard.WhiteMate, ref chessBoard.StaleMate);

            if (chessBoard.BlackMate || chessBoard.WhiteMate || chessBoard.StaleMate)
            {
                return true;
            }

            return false;
        }

        private static bool FindPlayBookMove(ref ChessMove bestMove, ChessBoard chessBoard, IEnumerable<OpeningMove> openingBook)
        {
            //Get the Hash for the current Board;
            string boardFen= ChessBoard.Fen(true, chessBoard);

            //Check the Opening Move Book
            foreach (OpeningMove move in openingBook)
            {
                if (move.StartingFEN.Contains(boardFen))
                {
                    int index = 0;

                    bestMove = move.Moves[index];
                    return true;
                }
            }

            return false;
        }

        public void Undo()
        {
            if (UndoChessBoard != null)
            {
                PieceTakenRemove(ChessBoard.LastMove);
                PieceTakenRemove(PreviousChessBoard.LastMove);

                ChessBoard = new ChessBoard(UndoChessBoard);
                CurrentGameBook = new List<OpeningMove>(UndoGameBook);

                ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
                PositionEvaluation.EvaluateBoardScore(ChessBoard);
            }
        }

        private static byte GetBoardIndex(byte boardColumn, byte boardRow)
        {
            return (byte)(boardColumn + (boardRow * 8));
        }

        public byte[] GetEnPassantMoves()
        {
            if (ChessBoard == null)
            {
                return null;
            }

            var returnArray = new byte[2];

            returnArray[0] = (byte)(ChessBoard.EnPassantPosition % 8);
            returnArray[1] = (byte)(ChessBoard.EnPassantPosition / 8);

            return returnArray;
        }

        public bool GetBlackMate()
        {
            if (ChessBoard == null)
            {
                return false;
            }

            return ChessBoard.BlackMate;
        }

        public bool GetWhiteMate()
        {
            return ChessBoard.WhiteMate;
        }

        public bool GetBlackCheck()
        {
            return ChessBoard.BlackCheck;
        }

        public bool GetWhiteCheck()
        {
            return ChessBoard.WhiteCheck;
        }

        public byte GetRepeatedMove()
        {
            return ChessBoard.RepeatedMove;
        }

        public byte GetHalfMoveClock()
        {
            return ChessBoard.HalfMoveClock;
        }

        public Stack<ChessMove> GetMoveHistory()
        {
            return MoveHistory;
        }

        public ChessPieceType GetPieceTypeAt(byte boardColumn, byte boardRow)
        {
            byte index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece == null)
            {
                return ChessPieceType.None;
            }

            return ChessBoard.Squares[index].Piece.PieceType;
        }

        public ChessPieceType GetPieceTypeAt(byte index)
        {
            if (ChessBoard.Squares[index].Piece == null)
            {
                return ChessPieceType.None;
            }

            return ChessBoard.Squares[index].Piece.PieceType;
        }

        public ChessPieceColor GetPieceColorAt(byte boardColumn, byte boardRow)
        {
            byte index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece == null)
            {
                return ChessPieceColor.White;
            }
            return ChessBoard.Squares[index].Piece.PieceColor;
        }

        public ChessPieceColor GetPieceColorAt(byte index)
        {
            if (ChessBoard.Squares[index].Piece == null)
            {
                return ChessPieceColor.White;
            }
            return ChessBoard.Squares[index].Piece.PieceColor;
        }

        public bool GetChessPieceSelected(byte boardColumn, byte boardRow)
        {
            byte index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece == null)
            {
                return false;
            }

            return ChessBoard.Squares[index].Piece.Selected;
        }

        public void GenerateValidMoves()
        {
            ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
        }

        public int EvaluateBoardScore()
        {
            PositionEvaluation.EvaluateBoardScore(ChessBoard);
            return ChessBoard.Score;
        }

        public byte[][] GetValidMoves(byte boardColumn, byte boardRow)
        {
            byte index = GetBoardIndex(boardColumn, boardRow);

            if (ChessBoard.Squares[index].Piece ==
                null)
            {
                return null;
            }

            var returnArray = new byte[ChessBoard.Squares[index].Piece.ValidMoves.Count][];
            int counter = 0;

            foreach (byte square in ChessBoard.Squares[index].Piece.ValidMoves)
            {
                returnArray[counter] = new byte[2];
                returnArray[counter][0] = (byte)(square % 8);
                returnArray[counter][1] = (byte)(square /8);
                counter++;
            }

            return returnArray;
        }

        public int GetScore()
        {
            return ChessBoard.Score;
        }

        public byte FindSourcePositon(ChessPieceType chessPieceType, ChessPieceColor chessPieceColor, byte dstPosition, bool capture, int forceCol, int forceRow)
        {
            VariableSquare square;

            if (dstPosition == ChessBoard.EnPassantPosition && chessPieceType == ChessPieceType.Pawn)
            {
                if (chessPieceColor == ChessPieceColor.White)
                {
                    square = ChessBoard.Squares[dstPosition + 7];

                    if (square.Piece != null)
                    {
                        if (square.Piece.PieceType == ChessPieceType.Pawn)
                        {
                            if (square.Piece.PieceColor == chessPieceColor)
                            {
                                if ((dstPosition + 7) % 8 == forceCol || forceCol == -1)
                                {
                                    return (byte)(dstPosition + 7);
                                }
                                
                            }
                        }
                    }

                    square = ChessBoard.Squares[dstPosition + 9];

                    if (square.Piece != null)
                    {
                        if (square.Piece.PieceType == ChessPieceType.Pawn)
                        {
                            if (square.Piece.PieceColor == chessPieceColor)
                            {
                                if ((dstPosition + 9) % 8 == forceCol || forceCol == -1)
                                {
                                    return (byte) (dstPosition + 9);
                                }
                            }
                        }
                    }
                }
                else 
                {
                    if (dstPosition - 7 >= 0)
                    {
                        square = ChessBoard.Squares[dstPosition - 7];

                        if (square.Piece != null)
                        {
                            if (square.Piece.PieceType == ChessPieceType.Pawn)
                            {
                                if (square.Piece.PieceColor == chessPieceColor)
                                {
                                    if ((dstPosition - 7)%8 == forceCol || forceCol == -1)
                                    {
                                        return (byte) (dstPosition - 7);
                                    }
                                }
                            }
                        }
                    }
                    if (dstPosition - 9 >= 0)
                    {
                        square = ChessBoard.Squares[dstPosition - 9];

                        if (square.Piece != null)
                        {
                            if (square.Piece.PieceType == ChessPieceType.Pawn)
                            {
                                if (square.Piece.PieceColor == chessPieceColor)
                                {
                                    if ((dstPosition - 9)%8 == forceCol || forceCol == -1)
                                    {
                                        return (byte) (dstPosition - 9);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (byte x = 0; x < 64; x++)
            {
                square = ChessBoard.Squares[x];

                if (square.Piece == null)
                    continue;
                if (square.Piece.PieceType != chessPieceType)
                    continue;
                if (square.Piece.PieceColor != chessPieceColor)
                    continue;
               
                foreach (byte move in square.Piece.ValidMoves)
                {
                    if (move == dstPosition)
                    {
                        if (!capture)
                        {
                            if ((byte)(x / 8) == (forceRow) || forceRow == -1)
                            {
                                if (x%8 == forceCol || forceCol == -1)
                                {
                                    return x;
                                }
                            }
                        }
                                
                        //Capture
                        if (ChessBoard.Squares[dstPosition].Piece != null)
                        {
                            if (ChessBoard.Squares[dstPosition].Piece.PieceColor != chessPieceColor)
                            {
                                if (x % 8 == forceCol || forceCol == -1)
                                {
                                    if ((byte)(x / 8) == (forceRow) || forceRow == -1)
                                    {
                                        return x;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

		public bool IsValidMoveAN(string move)
		{
			byte sourceColumn=0, sourceRow=0, destinationColumn=0, destinationRow=0;
			ChessMove.ParseAN(move, ref sourceColumn, ref sourceRow, ref destinationColumn, ref destinationRow);
			return IsValidMove(sourceColumn, sourceRow, destinationColumn, destinationRow);
		}

        public bool IsValidMove(byte srcPosition, byte dstPosition)
        {
            if (ChessBoard == null)
            {
                return false;
            }

            if (ChessBoard.Squares == null)
            {
                return false;
            }

            if (ChessBoard.Squares[srcPosition].Piece == null)
            {
                return false;
            }

            foreach (byte bs in ChessBoard.Squares[srcPosition].Piece.ValidMoves)
            {
                if (bs == dstPosition)
                {
                    return true;
                }
            }

            if (dstPosition == ChessBoard.EnPassantPosition)
            {
                return true;
            }

            return false;
        }

        public bool IsValidMove(byte sourceColumn, byte sourceRow, byte destinationColumn, byte destinationRow)
        {
            if (ChessBoard == null)
            {
                return false;
            }

            if (ChessBoard.Squares == null)
            {
                return false;
            }

            byte index = GetBoardIndex(sourceColumn, sourceRow);

            if (ChessBoard.Squares[index].Piece == null)
            {
                return false;
            }

            foreach (byte bs in ChessBoard.Squares[index].Piece.ValidMoves)
            {
                if (bs % 8 == destinationColumn)
                {
                    if ((byte)(bs / 8) == destinationRow)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsGameOver()
        {
            if (ChessBoard.StaleMate)
            {
                return true;
            }
            if (ChessBoard.WhiteMate || ChessBoard.BlackMate)
            {
                return true;
            }
            if (ChessBoard.HalfMoveClock >= 100)
            {
                return true;
            }
            if (ChessBoard.RepeatedMove >= 3)
            {
                return true;
            }

            if (ChessBoard.InsufficientMaterial)
            {
                return true;
            }
            return false;
        }

        public bool IsTie()
        {
            if (ChessBoard.StaleMate)
            {
                return true;
            }
            
            if (ChessBoard.HalfMoveClock >= 100)
            {
                return true;
            }
            if (ChessBoard.RepeatedMove >= 3)
            {
                return true;
            }

            if (ChessBoard.InsufficientMaterial)
            {
                return true;
            }

            return false;
        }
        public bool MovePiece(byte srcPosition, byte dstPosition)
        {
            ChessPiece piece = ChessBoard.Squares[srcPosition].Piece;

            PreviousChessBoard = new ChessBoard(ChessBoard);
            UndoChessBoard = new ChessBoard(ChessBoard);
            UndoGameBook = new List<OpeningMove>(CurrentGameBook);

            ChessBoard.MovePiece(ChessBoard, srcPosition, dstPosition, PromoteToPieceType);

            ChessBoard.LastMove.GeneratePGNString(ChessBoard);

            ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
            PositionEvaluation.EvaluateBoardScore(ChessBoard);

            //If there is a check in place, check if this is still true;
            if (piece.PieceColor == ChessPieceColor.White)
            {
                if (ChessBoard.WhiteCheck)
                {
                    //Invalid Move
                    ChessBoard = new ChessBoard(PreviousChessBoard);
                    ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
                    return false;
                }
            }
            else if (piece.PieceColor == ChessPieceColor.Black)
            {
                if (ChessBoard.BlackCheck)
                {
                    //Invalid Move
                    ChessBoard = new ChessBoard(PreviousChessBoard);
                    ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
                    return false;
                }
            }
   
            MoveHistory.Push(ChessBoard.LastMove);
           

            CheckForMate(WhoseMove, ref ChessBoard);
            PieceTakenAdd(ChessBoard.LastMove);

            if (ChessBoard.WhiteMate || ChessBoard.BlackMate)
            {
                LastMove.PgnMove += "#";
            }
            else if (ChessBoard.WhiteCheck || ChessBoard.BlackCheck)
            {
                LastMove.PgnMove += "+";
            }

            return true;
        }

        private void PieceTakenAdd(ChessMove lastMove)
        {
            if (lastMove.TakenPiece.PieceType != ChessPieceType.None)
            {
                if (lastMove.TakenPiece.PieceColor == ChessPieceColor.White)
                {
                    if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                    {
                        PiecesTakenCount.WhiteQueen++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                    {
                        PiecesTakenCount.WhiteRook++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                    {
                        PiecesTakenCount.WhiteBishop++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                    {
                        PiecesTakenCount.WhiteKnight++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn)
                    {
                        PiecesTakenCount.WhitePawn++;
                    }
                }
                if (ChessBoard.LastMove.TakenPiece.PieceColor == ChessPieceColor.Black)
                {
                    if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                    {
                        PiecesTakenCount.BlackQueen++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                    {
                        PiecesTakenCount.BlackRook++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                    {
                        PiecesTakenCount.BlackBishop++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                    {
                        PiecesTakenCount.BlackKnight++;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn)
                    {
                        PiecesTakenCount.BlackPawn++;
                    }
                }
            }
        }

        private void PieceTakenRemove(ChessMove lastMove)
        {
            if (lastMove.TakenPiece.PieceType != ChessPieceType.None)
            {
                if (lastMove.TakenPiece.PieceColor == ChessPieceColor.White)
                {
                    if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                    {
                        PiecesTakenCount.WhiteQueen--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                    {
                        PiecesTakenCount.WhiteRook--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                    {
                        PiecesTakenCount.WhiteBishop--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                    {
                        PiecesTakenCount.WhiteKnight--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn)
                    {
                        PiecesTakenCount.WhitePawn--;
                    }
                }
                if (lastMove.TakenPiece.PieceColor == ChessPieceColor.Black)
                {
                    if (lastMove.TakenPiece.PieceType == ChessPieceType.Queen)
                    {
                        PiecesTakenCount.BlackQueen--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Rook)
                    {
                        PiecesTakenCount.BlackRook--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Bishop)
                    {
                        PiecesTakenCount.BlackBishop--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Knight)
                    {
                        PiecesTakenCount.BlackKnight--;
                    }
                    else if (lastMove.TakenPiece.PieceType == ChessPieceType.Pawn)
                    {
                        PiecesTakenCount.BlackPawn--;
                    }
                }
            }
        }

		public bool MovePieceAN(string move)
		{
			byte sourceColumn=0, sourceRow=0, destinationColumn=0, destinationRow=0;
			ChessMove.ParseAN(move, ref sourceColumn, ref sourceRow, ref destinationColumn, ref destinationRow);
			return MovePiece(sourceColumn, sourceRow, destinationColumn, destinationRow);
		}

        public bool MovePiece(byte sourceColumn, byte sourceRow, byte destinationColumn, byte destinationRow)
        {
            byte srcPosition = (byte)(sourceColumn + (sourceRow * 8));
            byte dstPosition = (byte)(destinationColumn + (destinationRow * 8));

            return MovePiece(srcPosition, dstPosition);
        }

        internal void SetChessPiece(ChessPiece piece, byte index)
        {
            ChessBoard.Squares[index].Piece = new ChessPiece(piece);

        }

        #endregion

        #region Search

        public void AiPonderMove()
        {
            Thinking = true;

			ChessBoard.BlackMate = false;
			ChessBoard.WhiteMate = false;
			ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
		
            NodesSearched = 0;
			
			var resultBoards = new VariableResultBoards();
            resultBoards.Positions = new List<ChessBoard>();

            if (CheckForMate(WhoseMove, ref ChessBoard))
            {
                Thinking = false;
				return;
            }

            ChessMove bestMove = new ChessMove();
           
            //If there is no playbook move search for the best move
            if (FindPlayBookMove(ref bestMove, ChessBoard, OpeningBook) == false
                || ChessBoard.HalfMoveClock > 90 || ChessBoard.RepeatedMove >= 2)
            {
                if (FindPlayBookMove(ref bestMove, ChessBoard, CurrentGameBook) == false ||
                    ChessBoard.HalfMoveClock > 90 || ChessBoard.RepeatedMove >= 2)
                {
					bestMove = PositionSearch.IterativeSearch(ChessBoard, PlyDepthSearched, ref NodesSearched, ref NodesQuiessence, ref pvLine, ref PlyDepthReached, ref RootMovesSearched, CurrentGameBook);
                }
            }
 
            //Make the move 
            PreviousChessBoard = new ChessBoard(ChessBoard);

            RootMovesSearched = (byte)resultBoards.Positions.Count;

            ChessBoard.MovePiece(ChessBoard, bestMove.MovingPiecePrimary.SourcePosition, bestMove.MovingPiecePrimary.DestinatedPosition, ChessPieceType.Queen);

            ChessBoard.LastMove.GeneratePGNString(ChessBoard);
           

            for (byte x = 0; x < 64; x++)
            {
                VariableSquare sqr = ChessBoard.Squares[x];

                if (sqr.Piece == null)
                    continue;

                sqr.Piece.DefendedValue = 0;
                sqr.Piece.AttackedValue = 0;
            }

            ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
            PositionEvaluation.EvaluateBoardScore(ChessBoard);

            PieceTakenAdd(ChessBoard.LastMove);

            MoveHistory.Push(ChessBoard.LastMove);

            if (CheckForMate(WhoseMove, ref ChessBoard))
            {
                Thinking = false;

                if (ChessBoard.WhiteMate || ChessBoard.BlackMate)
                {
                    LastMove.PgnMove += "#";
                }
				
                return;
            }

            if (ChessBoard.WhiteCheck || ChessBoard.BlackCheck)
            {
                LastMove.PgnMove += "+";
            }

            Thinking = false;
		}

        public void HumanMove(byte source , byte destination) 
        {

            ChessMove move = new ChessMove();

            move.MovingPiecePrimary.PieceColor = ChessBoard.Squares[source].Piece.PieceColor;
            move.MovingPiecePrimary.PieceType = ChessBoard.Squares[source].Piece.PieceType;
            move.MovingPiecePrimary.SourcePosition = source;
            move.MovingPiecePrimary.DestinatedPosition = destination;

            ChessBoard.MovePiece(ChessBoard, move.MovingPiecePrimary.SourcePosition, move.MovingPiecePrimary.DestinatedPosition, ChessPieceType.Queen);


            ChessBoard.LastMove.GeneratePGNString(ChessBoard);

            for (byte x = 0; x < 64; x++)
            {
                VariableSquare sqr = ChessBoard.Squares[x];

                if (sqr.Piece == null)
                    continue;

                sqr.Piece.DefendedValue = 0;
                sqr.Piece.AttackedValue = 0;
            }

            ChessPieceValidMoves.GenerateValidMoves(ChessBoard);
            PositionEvaluation.EvaluateBoardScore(ChessBoard);

            PieceTakenAdd(ChessBoard.LastMove);

            MoveHistory.Push(ChessBoard.LastMove);

            if (CheckForMate(WhoseMove, ref ChessBoard))
            {
                Thinking = false;

                if (ChessBoard.WhiteMate || ChessBoard.BlackMate)
                {
                    LastMove.PgnMove += "#";
                }

                return;
            }

            if (ChessBoard.WhiteCheck || ChessBoard.BlackCheck)
            {
                LastMove.PgnMove += "+";
            }


        }

        #endregion


        #region FileIO

        public bool LoadOpeningBook()
        {
            OpeningBook = VariableOpeningBook.LoadOpeningBook();

            return true;
        }

        #endregion
   

    }
}
