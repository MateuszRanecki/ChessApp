//Signalr Logic

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/HistoryHub").build();



const elements =
{
    GameHistory: document.querySelector("#PlaceForGames"),
    ReviewSpace: document.querySelector("#ReviewSpace"),
    PlayButton: document.querySelector("#PlayButton"),
    Sequence: document.querySelector("#Sequence")
}

connection.start();

var CanMove = false;

connection.on("ShowPlayedGames", function (opponnent,date) {
    let div = document.createElement('div');
    let span = document.createElement('span');
    let button = document.createElement('button');
    button.setAttribute('id', name);
    button.setAttribute('class', 'btn btn-outline-dark bg-dark text-white ');
    button.innerText = "Sprawdź";
    button.addEventListener("click", function () {
        connection.invoke("ReviewGame",date)
    })
    span.innerText = "Przeciwko " + opponnent + "\n" + "Grana: " + date;
    div.setAttribute("class", "GameSpans");    
    div.append(span, button);
    elements.GameHistory.appendChild(div);
})

connection.on("CreateEnvForReview", function ()
{    
    playBoard.position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
    elements.ReviewSpace.innerText = '';
    elements.PlayButton.innerText = '';
    let div = document.createElement('div');
    let previous = document.createElement('button');
    let next = document.createElement('button');    
    previous.setAttribute('class', 'btn btn-outline-dark bg-dark text-white padding-20px');
    next.setAttribute('class', 'btn btn-outline-dark bg-dark text-white padding-20px');    
    previous.addEventListener("click", function ()
    {
        connection.invoke("UpdateMove", false);
    })
    next.addEventListener("click", function () {
        connection.invoke("UpdateMove", true);
    })  
    previous.innerText = "Poprzedni";
    next.innerText = "Następny";
    div.setAttribute("class", "ReviewButtons");   
    div.append(previous, next);
    elements.ReviewSpace.appendChild(div);

})

connection.on("ShowSequence", function (sequence)
{
    elements.Sequence.innerText = '';
    let span = document.createElement('span');
    span.innerText ="Sekwecja ruchów: \n"+ sequence;
    elements.Sequence.appendChild(span);
})


function StartAnalyze()
{
    game.fen();
    elements.ReviewSpace.innerText = '';
    elements.PlayButton.innerText = '';
    CanMove = true;
}

connection.on("UpdateBoard", function (counter)
{
    playBoard.position(counter);
    game.fen() = counter;
    updateStatus();
})

connection.on("OutOfRangeMoves", function (max)
{
    if (max == true) {
        alert("Osiągnięto pozycję końcową")
    }
    else
    {        
        alert("Osiągnięto pozycję startową");
    }
})



//Chess Logic

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
    if (CanMove == false)
    {
        return false;
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

function FlipBoard()
{
    playBoard.flip();
}

var playConfig = {
    draggable: true,
    position: 'start',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onSnapEnd: onSnapEnd,
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png'
}
playBoard = Chessboard('myBoard', playConfig)

updateStatus()