var connection = new signalR.HubConnectionBuilder().withUrl("/SetPositionHub").build();

connection.start();

//signalr logic

connection.on("ComputerStarts", function (move) {
    game.move(move);
    playBoard.position(game.fen())
    updateStatus()
})

function ShowOptions()
{
    var option = document.getElementById("howToPlay").value  
    var row = document.getElementById("hidden_row")

    if (option == "computer") row.style.visibility = "visible"
    else row.style.visibility="hidden"
}

//chess logic

var setBoard = Chessboard('posBoard', {
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png',
    draggable: true,
    dropOffBoard: 'trash',
    sparePieces: true
})

$('#startBtn').on('click', setBoard.start)
$('#clearBtn').on('click', setBoard.clear)

var boardValue;

function PlayPosition()
{   
    var playingBoardElement = document.getElementById("playBoard")
    var buttonsElement = document.getElementById("buttons")
    var gameDataElement = document.getElementById("gameData")
    var gameOptionsElement = document.getElementById("gameOptions")

    var whiteCastle = document.getElementById("white-castle").checked
    var blackCastle = document.getElementById("black-castle").checked
    var whiteLongCastle = document.getElementById("white-long-castle").checked
    var blackLongCastle = document.getElementById("white-long-castle").checked 

    var colorChoice = document.getElementById("colors").value   
    var playComputer = document.getElementById("howToPlay").value
    var difficulty = document.getElementById("level").value
    var playerColor = document.getElementById("playerColor").value   

    boardValue = setBoard.fen()
    var boardPosition = setBoard.position()

    var castling = SetCastlingRights(boardPosition, whiteCastle, blackCastle, whiteLongCastle, blackLongCastle)

    var finalFEN = SetFinalPosition(colorChoice, castling, boardValue)  

    game = new Chess(finalFEN)   
    playBoard.position(game.fen())
    updateStatus() 
  
    if (playComputer == "computer") connection.invoke("PlayWithComputer", finalFEN, difficulty, playerColor)    
    
   
    gameOptionsElement.remove()
    buttonsElement.remove()
    setBoard.destroy();   
   
    playingBoardElement.style.visibility = "visible"
    gameDataElement.style.visibility = "visible"    

}

function SetFinalPosition(color, castling,board)
{
    var moveOrder;
    var castleHelper = "-"    
    var result;

    if (color == "white") moveOrder = "w"
    else moveOrder = "b"

    if (castling !== "")
    {
        castleHelper = castling
    }

    result = board.concat(" ", moveOrder, " ", castleHelper, " - 0 1")

    return result
}

function SetCastlingRights(board, whiteCastleChecker, blackCastleChecker,whiteLongCastleChecker,blackLongCastleChecker)
{
    var canWhiteCastle;
    var canBlackCastle;
    var canWhiteLongCastle;
    var canBlackLongCastle;
    var helper = "";
    var result="";   

    if (whiteCastleChecker)
    {
        canWhiteCastle = CheckWhiteCastle(board)
        if (canWhiteCastle != null) result += canWhiteCastle
    }

    if (whiteLongCastleChecker) {
        canWhiteLongCastle = CheckWhiteLongCastle(board)
        if (canWhiteLongCastle !== null) result += canWhiteLongCastle
    }

    if (blackCastleChecker) {
        canBlackCastle = CheckBlackCastle(board)
        if (canBlackCastle !== null) result += canBlackCastle
    }

    if (blackLongCastleChecker) {
        canBlackLongCastle = CheckBlackLongCastle(board)
        if (canBlackLongCastle !== null) result += canBlackLongCastle
    }  

    return result
}

function CheckWhiteCastle(board)
{ 
    if (board["e1"] === "wK" && board["h1"] === "wR") {        
        return "K"
    }
    else return null
}

function CheckBlackCastle(board) {    
    if (board["e8"] === "bK" && board["h8"] === "bR") {
        return "k"
    }
    else return null
}

function CheckWhiteLongCastle(board) {    
    if (board["e1"] === "wK" && board["a1"] === "wR") {
        return "Q"
    }
    else return null
}

function CheckBlackLongCastle(board) {    
    if (board["e8"] === "bK" && board["a8"] === "bR") {
        return "q"
    }
    else return null
}



var playBoard = null
var game = new Chess()
var $status = $('#status')
var $fen = $('#fen')
var $pgn = $('#pgn')

function onDragStart(source, piece, position, orientation) {
    // do not pick up pieces if the game is over
    if (game.game_over()) return false

    // only pick up pieces for the side to move
    if ((game.turn() === 'w' && piece.search(/^b/) !== -1) ||
        (game.turn() === 'b' && piece.search(/^w/) !== -1)) {
        return false
    }
}

function onDrop(source, target) {
    // see if the move is legal
    var move = game.move({
        from: source,
        to: target,
        promotion: 'q' // NOTE: always promote to a queen for example simplicity
    })

    // illegal move
    if (move === null) return 'snapback'

    updateStatus()

    connection.invoke("MakeHumanMove", source, target)
}

// update the board position after the piece snap
// for castling, en passant, pawn promotion
function onSnapEnd() {
    playBoard.position(game.fen())
}

function updateStatus() {
    var status = ''

    var moveColor = 'White'
    if (game.turn() === 'b') {
        moveColor = 'Black'
    }

    // checkmate?
    if (game.in_checkmate()) {
        status = 'Game over, ' + moveColor + ' is in checkmate.'
    }

    // draw?
    else if (game.in_draw()) {
        status = 'Game over, drawn position'
    }

    // game still on
    else {
        status = moveColor + ' to move'

        // check?
        if (game.in_check()) {
            status += ', ' + moveColor + ' is in check'
        }
    }

    $status.html(status)
    $fen.html(game.fen())
    $pgn.html(game.pgn())
}


var playConfig = {
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png',
    draggable: true,
    position: 'start',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onSnapEnd: onSnapEnd
}

playBoard = Chessboard('playBoard', playConfig)

updateStatus()

