namespace Chess_App
{
    
    internal struct VariableSquare
    {
        internal ChessPiece Piece;

        #region Constructors

        internal VariableSquare(ChessPiece piece)
        {
            Piece = new ChessPiece(piece);
        }

        #endregion
    }
}