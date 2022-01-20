"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ComputerHub").build();

connection.start();


//SignalR logic

connection.on("ComputerMoved", function (engineMove) {
    game.move(engineMove)
    playBoard.position(game.fen()) 
})

connection.on("Flip", function () {
    playBoard.flip();
})

var color;

function PlayGame()
{
    var diff = document.getElementById("level").value
    var colorValue = document.getElementById("playerColor").value;

    if (colorValue == "white") color = "w"
    else color="b"

    document.getElementById("Board").style.display = 'block'
    document.getElementById("ColorPick").style.display = 'none'
    connection.invoke("ComputerStartsGame", diff, colorValue);     
}

var playBoard = null
var game = new Chess()

function onDragStart(source, piece, position, orientation) {
    // do not pick up pieces if the game is over
    if (game.game_over()) return false

    // only pick up pieces for White
    if (color !== null)
    {
        if (color === "w") {
            if (piece.search(/^b/) !== -1) return false
            if (game.turn === "b") return false
        }
        else
        {
            if (piece.search(/^w/) !== -1) return false
            if (game.turn === "b") return false
        }
    }
     
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

        connection.invoke("PlayerMoved", source, target)      
    }  
}

// update the board position after the piece snap
// for castling, en passant, pawn promotion
function onSnapEnd() {
    playBoard.position(game.fen())
}

var playConfig = {
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png',
    draggable: true,
    position: 'start',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onSnapEnd: onSnapEnd
}
playBoard = Chessboard('myBoard', playConfig)

window.addEventListener('resize', function () {
    playBoard.resize;
})