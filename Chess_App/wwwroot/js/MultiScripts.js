"use strict";



var connection = new signalR.HubConnectionBuilder().withUrl("/ChessmultiHub").build();
const elements =
{
    PlayersList: document.querySelector("#PlayersList"),
    DrawButtons: document.querySelector("#DrawButtons"),
    ChatShow: document.querySelector("#ChatShow"),
    chatPlace: document.querySelector("#chatSpace")
}

//SignalR logic

connection.start();


connection.on("ShowLoggedPlayers", function (name) {
    let div = document.createElement('div');
    let span = document.createElement('span');
    span.innerText = name;
    div.append(span);
    elements.PlayersList.appendChild(div);
})

var canMove = null;
var FenFromGame = null;

connection.on("canplayermove", function (color) {
    if (color == "w") {
        canMove = true;

    }
    else if (color == "b") {
        canMove = false;

    }
})

connection.on("OverByResign", function (color) {    
    canMove = false;
    var status = ''
    if (color == "w") {
        status = 'Game over. White resigned'
    }
    else if (color == "b")
    {
        status = 'Game over. Black resigned'
    }
    $status.html(status)
    GetFenFromGame()
})

function OfferDraw() {    
    connection.invoke("SendDrawOffer");
}

connection.on("ReceiveDrawOffer", function (fromWho)
{    
    let div = document.createElement('div');
    let span = document.createElement('span');
    let accept = document.createElement('button');
    let decline = document.createElement('button'); 
    accept.setAttribute('class', 'btn btn-outline-dark bg-dark text-white');
    accept.innerText = "Przyjmij";
    accept.addEventListener("click", function () {
        Acceptoffer();
    })
    decline.setAttribute('class', 'btn btn-outline-dark bg-dark text-white');
    decline.innerText = "Odmów";
    decline.addEventListener("click", function () {
        Declineoffer();
    })
    span.innerText = fromWho + ' proponuje remis';
    div.setAttribute('id','OptionButtons')
    div.append(span, accept, decline);
    elements.DrawButtons.appendChild(div);

})


function Acceptoffer()
{
    connection.invoke("DrawOfferAccepted");    

}

connection.on("ShowDraw", function ()
{
    canMove = false;
    var status = 'Game drawn by agreement';
    $status.html(status)
    GetFenFromGame()
})

function Declineoffer()
{
    var buttons = document.getElementById('OptionButtons');
    elements.DrawButtons.removeChild(buttons)
}


function MessageSent()
{
    var content = document.getElementById("ChatInput").value;
    var clear = document.getElementById("ChatInput");
    clear.value = '';
    connection.invoke("SendMessageToAll", content);    
}

connection.on("ReceiveMessageToAll", function (message, sender)
{
    let content = document.getElementById('ChatShow');
    content.innerText += sender +' : ' + message + '\n';

})

connection.on("ClearList", function ()
{
    const node = document.getElementById("PlayersList");
    node.innerHTML = '';
})

function GetFenFromGame()
{    
    var moves = game.pgn();
    connection.invoke('SaveFenFromGame', FenFromGame, moves);
}


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
    if (canMove == false) {
        return false
    }

}

function onDrop(source, target) {
    connection.invoke("MakeMove", source, target);
    // see if the move is legal
    var move = game.move({
        from: source,
        to: target,
        promotion: 'q' // NOTE: always promote to a queen for example simplicity
    })

    // illegal move
    if (move === null) return 'snapback'
    ChangePLayersMoveOrder()
    updateStatus()
}

connection.on("UpdateBoard", function (source, target) {
    var move = game.move({
        from: source,
        to: target,
        promotion: 'q' // NOTE: always promote to a queen for example simplicity
    })

    // illegal move
    if (move === null) return 'snapback'

    onSnapEnd();
    ChangePLayersMoveOrder()
    updateStatus()

})

function ChangePLayersMoveOrder() {
    if (canMove == true) {
        canMove = false;
    }
    else {
        canMove = true;
    }
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
        ShowFen();
        GetFenFromGame()
    }

    // draw?
    else if (game.in_draw()) {
        status = 'Game over, drawn position'
        ShowFen();
        GetFenFromGame();
    }

    // game still on
    else {
        status = moveColor + ' to move'

        // check?
        if (game.in_check()) {
            status += ', ' + moveColor + ' is in check'
        }
    }
    ShowFen();
    $status.html(status)
    $fen.html(game.fen())
    $pgn.html(game.pgn())
}

function ShowFen()
{
    FenFromGame += game.fen() + ';';
}

function FlipBoard() {
    playBoard.flip();
}

function ResignGame() {

    var whichMove = canMove;
    var color = null;

    if (game.turn() === "w" && canMove == true ||
        game.turn() === "b" && canMove == false)
        {
        color = "w";
        }
    else if (game.turn() === "b" && canMove == true ||
        game.turn() === "w" && canMove == false)
         {
        color = "b";
         }

    connection.invoke("PlayerResigned", color);
}


var playConfig =
{
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png',
    draggable: true,
    position: 'start',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onSnapEnd: onSnapEnd
}
playBoard = Chessboard('MultiBoard', playConfig)


updateStatus()