using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Chess_App
{
    internal static class PositionSearch
    {
        internal static int progress;
		
		private static int piecesRemaining;
		

        private struct Position
        {
            internal byte SourcePosition;
            internal byte DestinatedPosition;
            internal int GameScore;          
            internal string Move;

            public new string ToString()
            {
                return Move;
            }

        }

        private static readonly Position[,] KillerMove = new Position[3,20];
        private static int killerIndex;

        private static int Sort(Position s2, Position s1)
        {
            return (s1.GameScore).CompareTo(s2.GameScore);
        }

        private static int Sort(ChessBoard s2, ChessBoard s1)
        {
            return (s1.Score).CompareTo(s2.Score);
        }

        private static int SideToMoveScore(int score, ChessPieceColor color)
        {
            if (color == ChessPieceColor.Black)
                return -score;

            return score;
        }

       

        internal static ChessMove IterativeSearch(ChessBoard examinedBoard, byte depth, ref int nodesSearched, ref int nodesQuiessence,
            ref string pvLine, ref byte plyDepthReached, ref byte rootMovesSearched, List<OpeningMove> bookMove)
        {
            List<Position> pvChild = new List<Position>();
            int alpha = -400000000;
            const int beta = 400000000;
			
            
            ChessMove bestMove = new ChessMove();

            //We are going to store our result boards here           
            VariableResultBoards resultBoards = GetSortValidMoves(examinedBoard);

            rootMovesSearched = (byte)resultBoards.Positions.Count;

            if (rootMovesSearched == 1)
            {
                //I only have one move
                return resultBoards.Positions[0].LastMove;
            }

            //Can I make an instant mate?
            foreach (ChessBoard position in resultBoards.Positions)
            {
                int value = -AlphaBeta(position, 1, -beta, -alpha, ref nodesSearched, ref nodesQuiessence, ref pvChild, true);

                if (value >= 32767)
                {
                    return position.LastMove;
                }
            }

            int currentBoard = 0;

            alpha = -400000000;

            resultBoards.Positions.Sort(Sort);

            depth--;

            plyDepthReached = ModifyDepth(depth, resultBoards.Positions.Count);

            foreach (ChessBoard position in resultBoards.Positions)
            {
                currentBoard++;

				progress = (int)((currentBoard / (decimal)resultBoards.Positions.Count) * 100);

                pvChild = new List<Position>();

                int value = -AlphaBeta(position, depth, -beta, -alpha, ref nodesSearched, ref nodesQuiessence, ref pvChild, false);

                if (value >= 32767)
                {
                    return position.LastMove;
                }

                if (examinedBoard.RepeatedMove == 2)
                {
                    string fen = ChessBoard.Fen(true, position);

                    foreach (OpeningMove move in bookMove)
                    {
                        if (move.EndingFEN == fen)
                        {
                            value = 0;
                            break;
                        }
                    }
                }

                position.Score = value;

                //If value is greater then alpha this is the best board
                if (value > alpha || alpha == -400000000)
                {
                    pvLine = position.LastMove.ToString();

                    foreach (Position pvPos in pvChild)
                    {
                        pvLine += " " + pvPos.ToString();
                    }

                    alpha = value;
                    bestMove = position.LastMove;
                }
            }

            plyDepthReached++;
			progress=100;
		
            return bestMove;
        }

        private static VariableResultBoards GetSortValidMoves(ChessBoard examineBoard)
        {
            VariableResultBoards succ = new VariableResultBoards
            {
                Positions = new List<ChessBoard>(30)
            };

            piecesRemaining = 0;

            for (byte x = 0; x < 64; x++)
            {
                VariableSquare sqr = examineBoard.Squares[x];

                //Make sure there is a piece on the square
                if (sqr.Piece == null)
                    continue;

                piecesRemaining++;

                //Make sure the color is the same color as the one we are moving.
                if (sqr.Piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (byte dst in sqr.Piece.ValidMoves)
                {
                    //We make copies of the board and move so that we can move it without effecting the parent board
                    ChessBoard board = examineBoard.FastCopy();

                    //Make move so we can examine it
                    ChessBoard.MovePiece(board, x, dst, ChessPieceType.Queen);

                    //We Generate Valid Moves for Board
                    ChessPieceValidMoves.GenerateValidMoves(board);

                    //Invalid Move
                    if (board.WhiteCheck && examineBoard.WhoseMove == ChessPieceColor.White)
                    {
                        continue;
                    }

                    //Invalid Move
                    if (board.BlackCheck && examineBoard.WhoseMove == ChessPieceColor.Black)
                    {
                        continue;
                    }

                    //We calculate the board score
                    PositionEvaluation.EvaluateBoardScore(board);

                    //Invert Score to support Negamax
                    board.Score = SideToMoveScore(board.Score, board.WhoseMove);

                    succ.Positions.Add(board);
                }
            }

            succ.Positions.Sort(Sort);
            return succ;
        }

        private static int AlphaBeta(ChessBoard examinedBoard, byte depth, int alpha, int beta, 
            ref int nodesSearched, ref int nodesQuiessence, ref List<Position> pvLine, bool extended)
        {
            nodesSearched++;

            if (examinedBoard.HalfMoveClock >= 100 || examinedBoard.RepeatedMove >= 3)
                return 0;
            
            if (depth == 0)
            {
                if (!extended && examinedBoard.BlackCheck || examinedBoard.WhiteCheck)
                {
                    depth++;
                    extended = true;
                }
                else
                {                    
                    return Quiescence(examinedBoard, alpha, beta, ref nodesQuiessence);
                }
            }

            List<Position> positions = EvaluateMoves(examinedBoard, depth);

            if (examinedBoard.WhiteCheck || examinedBoard.BlackCheck || positions.Count == 0)
            {
                if (SearchForMate(examinedBoard.WhoseMove, examinedBoard, ref examinedBoard.BlackMate,
                    ref examinedBoard.WhiteMate, ref examinedBoard.StaleMate))
                {
                    if (examinedBoard.BlackMate)
                    {
                        if (examinedBoard.WhoseMove == ChessPieceColor.Black)
                            return -32767-depth;

                        return 32767 + depth;
                    }
                    if (examinedBoard.WhiteMate)
                    {
                        if (examinedBoard.WhoseMove == ChessPieceColor.Black)
                            return 32767 + depth;

                        return -32767 - depth;
                    }
                    
                    return 0;
                }
            }

            positions.Sort(Sort);

            foreach (Position move in positions)
            {
                List<Position> pvChild = new List<Position>();             
                ChessBoard helpBoard = examinedBoard.FastCopy();               
                ChessBoard.MovePiece(helpBoard, move.SourcePosition, move.DestinatedPosition, ChessPieceType.Queen);                
                ChessPieceValidMoves.GenerateValidMoves(helpBoard);

                if (helpBoard.BlackCheck)
                {
                    if (examinedBoard.WhoseMove == ChessPieceColor.Black)
                    {                        
                        continue;
                    }
                }
                if (helpBoard.WhiteCheck)
                {
                    if (examinedBoard.WhoseMove == ChessPieceColor.White)
                    {                       
                        continue;
                    }
                }

                int value = -AlphaBeta(helpBoard, (byte)(depth - 1), -beta, -alpha,
                    ref nodesSearched, ref nodesQuiessence, ref pvChild, extended);

                if (value >= beta)
                {
                    KillerMove[killerIndex, depth].SourcePosition = move.SourcePosition;
                    KillerMove[killerIndex, depth].DestinatedPosition = move.DestinatedPosition;
                    killerIndex = ((killerIndex + 1) % 2);                    
                    return beta;
                }
                if (value > alpha)
                {
                    Position pvPos = new Position();
                    pvPos.SourcePosition = helpBoard.LastMove.MovingPiecePrimary.SourcePosition;
                    pvPos.DestinatedPosition = helpBoard.LastMove.MovingPiecePrimary.DestinatedPosition;
                    pvPos.Move = helpBoard.LastMove.ToString();
                    pvChild.Insert(0, pvPos);
                    pvLine = pvChild;
                    alpha = (int)value;
                }
            }
            return alpha;
        }

        private static int Quiescence(ChessBoard examineBoard, int alpha, int beta, ref int nodesSearched)
        {
            nodesSearched++;

            //Evaluate Score
            PositionEvaluation.EvaluateBoardScore(examineBoard);

            //Invert Score to support Negamax
            examineBoard.Score = SideToMoveScore(examineBoard.Score, examineBoard.WhoseMove);

            if (examineBoard.Score >= beta)
                return beta;

            if (examineBoard.Score > alpha)
                alpha = examineBoard.Score;

            
            List<Position> positions;
          

            if (examineBoard.WhiteCheck || examineBoard.BlackCheck)
            {
                positions = EvaluateMoves(examineBoard, 0);
            }
            else
            {
                positions = EvaluateMovesCapture(examineBoard);    
            }

            if (positions.Count == 0)
            {
                return examineBoard.Score;
            }
            
            positions.Sort(Sort);

            foreach (Position move in positions)
            {
                if (StaticExchangeEvaluation(examineBoard.Squares[move.DestinatedPosition]) >= 0)
                {
                    continue;
                }

                //Make a copy
                ChessBoard board = examineBoard.FastCopy();

                //Move Piece
                ChessBoard.MovePiece(board, move.SourcePosition, move.DestinatedPosition, ChessPieceType.Queen);

                //We Generate Valid Moves for Board
                ChessPieceValidMoves.GenerateValidMoves(board);

                if (board.BlackCheck)
                {
                    if (examineBoard.WhoseMove == ChessPieceColor.Black)
                    {
                        //Invalid Move
                        continue;
                    }
                }

                if (board.WhiteCheck)
                {
                    if (examineBoard.WhoseMove == ChessPieceColor.White)
                    {
                        //Invalid Move
                        continue;
                    }
                }

                int value = -Quiescence(board, - beta, -alpha, ref nodesSearched);

                if (value >= beta)
                {
                    KillerMove[2, 0].SourcePosition = move.SourcePosition;
                    KillerMove[2, 0].DestinatedPosition = move.DestinatedPosition;

                    return beta;
                }
                if (value > alpha)
                {
                    alpha = value;
                }
            }

            return alpha;
        }

        private static List<Position> EvaluateMoves(ChessBoard examineBoard, byte depth)
        {

            //We are going to store our result boards here           
            List<Position> positions = new List<Position>();            


            for (byte x = 0; x < 64; x++)
            {
                ChessPiece piece = examineBoard.Squares[x].Piece;

                //Make sure there is a piece on the square
                if (piece == null)
                    continue;

                //Make sure the color is the same color as the one we are moving.
                if (piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (byte dst in piece.ValidMoves)
                {
                    Position move = new Position();

                    move.SourcePosition = x;
                    move.DestinatedPosition = dst;
				
                    if (move.SourcePosition == KillerMove[0, depth].SourcePosition && move.DestinatedPosition == KillerMove[0, depth].DestinatedPosition)
                    {                        
                        move.GameScore += 5000;
                        positions.Add(move);
                        continue;
                    }
                    if (move.SourcePosition == KillerMove[1, depth].SourcePosition && move.DestinatedPosition == KillerMove[1, depth].DestinatedPosition)
                    {                        
                        move.GameScore += 5000;
                        positions.Add(move);
                        continue;
                    }

                    ChessPiece pieceAttacked = examineBoard.Squares[move.DestinatedPosition].Piece;

                    //If the move is a capture add it's value to the score
                    if (pieceAttacked != null)
                    {
                        move.GameScore += pieceAttacked.PieceValue;

                        if (piece.PieceValue < pieceAttacked.PieceValue)
                        {
                            move.GameScore += pieceAttacked.PieceValue - piece.PieceValue;
                        }
                    }

                    if (!piece.Moved)
                    {
                        move.GameScore += 10;
                    }

                    move.GameScore += piece.PieceActionValue;

                    //Add Score for Castling
                    if (!examineBoard.WhiteCastled && examineBoard.WhoseMove == ChessPieceColor.White)
                    {

                        if (piece.PieceType == ChessPieceType.King)
                        {
                            if (move.DestinatedPosition != 62 && move.DestinatedPosition != 58)
                            {
                                move.GameScore -= 40;
                            }
                            else
                            {
                                move.GameScore += 40;
                            }
                        }
                        if (piece.PieceType == ChessPieceType.Rook)
                        {
                            move.GameScore -= 40;
                        }
                    }

                    if (!examineBoard.BlackCastled && examineBoard.WhoseMove == ChessPieceColor.Black)
                    {
                        if (piece.PieceType == ChessPieceType.King)
                        {
                            if (move.DestinatedPosition != 6 && move.DestinatedPosition != 2)
                            {
                                move.GameScore -= 40;
                            }
                            else
                            {
                                move.GameScore += 40;
                            }
                        }
                        if (piece.PieceType == ChessPieceType.Rook)
                        {
                            move.GameScore -= 40;
                        }
                    }

                    positions.Add(move);
                }
            }

            return positions;
        }

        private static List<Position> EvaluateMovesCapture(ChessBoard examineBoard)
        {
            //We are going to store our result boards here           
            List<Position> positions = new List<Position>();

            for (byte x = 0; x < 64; x++)
            {
                ChessPiece piece = examineBoard.Squares[x].Piece;

                //Make sure there is a piece on the square
                if (piece == null)
                    continue;

                //Make sure the color is the same color as the one we are moving.
                if (piece.PieceColor != examineBoard.WhoseMove)
                    continue;

                //For each valid move for this piece
                foreach (byte dst in piece.ValidMoves)
                {
                    if (examineBoard.Squares[dst].Piece == null)
                    {
                        continue;
                    }

                    Position move = new Position();

                    move.SourcePosition = x;
                    move.DestinatedPosition = dst;

                    if (move.SourcePosition == KillerMove[2, 0].SourcePosition && move.DestinatedPosition == KillerMove[2, 0].DestinatedPosition)
                    {                        
                        move.GameScore += 5000;
                        positions.Add(move);
                        continue;
                    }

                    ChessPiece pieceAttacked = examineBoard.Squares[move.DestinatedPosition].Piece;

                    move.GameScore += pieceAttacked.PieceValue;

                    if (piece.PieceValue < pieceAttacked.PieceValue)
                    {
                        move.GameScore += pieceAttacked.PieceValue - piece.PieceValue;
                    }

                    move.GameScore += piece.PieceActionValue;


                    positions.Add(move);
                }
            }

            return positions;
        }

        internal static bool SearchForMate(ChessPieceColor movingSide, ChessBoard examineBoard, ref bool blackMate, ref bool whiteMate, ref bool staleMate)
        {
            bool foundNonCheckBlack = false;
            bool foundNonCheckWhite = false;

            for (byte x = 0; x < 64; x++)
            {
                VariableSquare sqr = examineBoard.Squares[x];

                //Make sure there is a piece on the square
                if (sqr.Piece == null)
                    continue;

                //Make sure the color is the same color as the one we are moving.
                if (sqr.Piece.PieceColor != movingSide)
                    continue;

                //For each valid move for this piece
                foreach (byte dst in sqr.Piece.ValidMoves)
                {

                    //We make copies of the board and move so that we can move it without effecting the parent board
                    ChessBoard board = examineBoard.FastCopy();

                    //Make move so we can examine it
                    ChessBoard.MovePiece(board, x, dst, ChessPieceType.Queen);

                    //We Generate Valid Moves for Board
                    ChessPieceValidMoves.GenerateValidMoves(board);

                    if (board.BlackCheck == false)
                    {
                        foundNonCheckBlack = true;
                    }
                    else if (movingSide == ChessPieceColor.Black)
                    {
                        continue;
                    }

                    if (board.WhiteCheck == false )
                    {
                        foundNonCheckWhite = true;
                    }
                    else if (movingSide == ChessPieceColor.White)
                    {
                        continue;
                    }
                }
            }

            if (foundNonCheckBlack == false)
            {
                if (examineBoard.BlackCheck)
                {
                    blackMate = true;
                    return true;
                }
                if (!examineBoard.WhiteMate && movingSide != ChessPieceColor.White)
                {
                    staleMate = true;
                    return true;
                }
            }

            if (foundNonCheckWhite == false)
            {
                if (examineBoard.WhiteCheck)
                {
                    whiteMate = true;
                    return true;
                }
                if (!examineBoard.BlackMate && movingSide != ChessPieceColor.Black)
                {
                    staleMate = true;
                    return true;
                }
            }

            return false;
           
        }

        private static byte ModifyDepth(byte depth, int possibleMoves)
        {
            if (possibleMoves <= 20 || piecesRemaining < 14)
            {
                if (possibleMoves <= 10 || piecesRemaining < 6)
                {
                    depth += 1;
                }

                depth += 1;
            }

            return depth;
        }

        private static int StaticExchangeEvaluation(VariableSquare examineSquare)
        {
            if (examineSquare.Piece == null)
            {
                return 0;
            }
            if (examineSquare.Piece.AttackedValue == 0)
            {
                return 0;
            }

            return examineSquare.Piece.PieceActionValue - examineSquare.Piece.AttackedValue + examineSquare.Piece.DefendedValue;
        }

    }
}
