"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ComputerHub").build();

connection.start();


//SignalR logic

connection.on("connected", function (msg)
{
    alert(msg)
    
})



//chess logic
var color;

function PlayGame(value)
{    
    color = value;
    document.getElementById("Board").style.display = 'block';
    document.getElementById("ColorPick").style.display = 'none';
    if (value === 'b')
    {
        board.flip();
        game.move("e4")
        board.position(game.fen())
    }
}

var board = null
var game = new Chess()

function onDragStart(source, piece, position, orientation) {
    // do not pick up pieces if the game is over
    if (game.game_over()) return false

    // only pick up pieces for White
    // if (piece.search(/^b/) !== -1) return false
}

function makeRandomMove() {
    var possibleMoves = game.moves()

    // game over
    if (possibleMoves.length === 0) return

    var randomIdx = Math.floor(Math.random() * possibleMoves.length)
    var move = possibleMoves[randomIdx];    
    game.move(possibleMoves[randomIdx])
    board.position(game.fen())
}

function onDrop(source, target) { 

    if (game.turn() === color)
    {
        // see if the move is legal
        var move = game.move({
            from: source,
            to: target,
            promotion: 'q' // NOTE: always promote to a queen for example simplicity
        })

        // illegal move
        if (move === null) return 'snapback' 

        //alert(target)
        
        // computer makes a move
        window.setTimeout(makeRandomMove, 250)
    }  
}

// update the board position after the piece snap
// for castling, en passant, pawn promotion
function onSnapEnd() {
    board.position(game.fen())
}

var config = {
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png',
    draggable: true,
    position: 'start',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onSnapEnd: onSnapEnd
}
board = Chessboard('myBoard', config)

