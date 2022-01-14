
var header = document.getElementById("header");
var topic = document.getElementById("topic");

function ShowMoves()
{
    var moves = document.getElementById("moveButtons")
    if (moves.style.visibility == "visible")
        moves.style.visibility = "hidden"
    else moves.style.visibility = "visible"

    header.innerHTML = "Poruszanie się bierek"
    topic.innerHTML ="w tej zakładce omówione jest poruszanie się bierek. Dla ułatwienia dostępne ruchy dla każdej figury są podświetlane po najechaniu na nie"
}

function SpecialMoves()
{
    var moves = document.getElementById("SpecialMoveButtons")
    if (moves.style.visibility == "visible")
        moves.style.visibility = "hidden"
    else moves.style.visibility = "visible"

    header.innerHTML = "Ruchy specjalne"
    topic.innerHTML = "w tej zakładce omówione są ruchy specjalne"
}

function ShowRules() {
    var moves = document.getElementById("rulesButtons")
    if (moves.style.visibility == "visible")
        moves.style.visibility = "hidden"
    else moves.style.visibility = "visible"

    header.innerHTML = "Zasady gry"
    topic.innerHTML = "w tej zakładce omówione są zasady gry"
}

function DrawOptions() {
    var moves = document.getElementById("drawButtons")
    if (moves.style.visibility == "visible")
        moves.style.visibility = "hidden"
    else moves.style.visibility = "visible"

    header.innerHTML = "Możliwości remisu"
    topic.innerHTML = "w tej zakładce omówione są możliwości remisu"
}

function Pawn()
{
    game = new Chess('1k6/8/p7/8/8/1P6/8/4K3 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Pion"
    topic.innerHTML = "Poruszają się tylko o jedno pole naprzód. Każdy pionek, który nie wykonał jeszcze posunięcia może wyjątkowo ruszyć się o 2 pola"
}

function Knight() {
    game = new Chess('1k6/1n6/8/8/8/8/6NP/4K3 w - - 0 1')
    board.position(game.fen())
    header.innerHTML = "Skoczek"
    topic.innerHTML="Skoczek może poruszyć się na jedno z najbliższych nie sąsiadujących z nim pól, które nie znajduje się ani na linii prostej, ani ukośnej. Mimo że sama definicja wygląda na skomplikowaną to istnieje łatwy sposób na przyswojenie sobie ruchów skoczkiem dzięki wierszykowi: „jeden, dwa na trzy w bok – taki jest konika skok”"
}

function Bishop() {
    game = new Chess('1k4b1/8/8/8/8/8/6BP/4K3 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Goniec"
    topic.innerHTML = "Porusza się i zbija o dowolną ilość pól po liniach ukośnych. Nie może przeskoczyć przez bierki własne lub przeciwnika"
}


function Rook() {
    game = new Chess('1k6/3r4/8/8/8/8/7R/3K4 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Wieża"
    topic.innerHTML ="Porusza się i zbija o dowolną ilość pól po liniach prostych. Nie może przeskoczyć przez bierki własne lub przeciwnika"
}

function Queen() {
    game = new Chess('1kq5/8/8/8/8/8/5Q2/3K4 w - - 0 1')
    board.position(game.fen())

    topic.innerHTML = "Porusza się i zbija o dowolną ilość pól po liniach prostych i ukośnych . Nie może przeskoczyć przez bierki własne lub przeciwnika"
    header.innerHTML = "Hetman"
}

function King() {
    game = new Chess('1k6/8/8/8/8/8/7P/3K4 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Król"
    topic.innerHTML ="Król porusza się i zbija w każdym kierunku o jedno pole – zarówno po liniach prostych jak i na ukos"
}

function EnPassant() {
    game = new Chess('1k6/8/8/pP6/8/8/8/3K4 w - a6 0 1')
    board.position(game.fen())

    header.innerHTML = "Bicie w przelocie"
    topic.innerHTML ="Możliwość bicia w przelocie powstaje tylko wówczas, gdy pion jednej ze stron wykonał ruch o dwa pola, a pole, które minął, jest atakowane przez piona strony przeciwnej. Pion bijący w przelocie zajmuje to właśnie wolne pole, które minął pion zbijany"
}

function Castle() {
    game = new Chess('1k6/8/8/8/8/8/8/R3K2R w KQ a6 0 1')
    board.position(game.fen())

    header.innerHTML="Roszada"
    topic.innerHTML ="Roszada polega na przesunięciu króla o dwa pola w kierunku wieży, a następnie ustawieniu wieży na polu, które minął król.Roszadę można wykonać, jeśli król nie wykonał ruchu od początku partii, wieża uczestnicząca w roszadzie nie wykonała ruchu od początku partii, pomiędzy królem i tą wieżą nie ma innych bierek, król nie jest szachowany, pole, przez które przejdzie król nie jest atakowane przez bierki przeciwnika, roszada nie spowoduje, że król znajdzie się pod szachem"
}


function Promotion() {
    game = new Chess('1k6/7P/8/8/8/8/8/4K3 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Promocja"
    topic.innerHTML ="Pion, który przemaszerował przez całą szachownicę i osiągnął ostatnią linię, musi być w tym samym ruchu zastąpiony hetmanem, wieżą, gońcem lub skoczkiem tego samego koloru. Taki ruch nazywa się promocją. Zazwyczaj pion jest promowany na najsilniejszą figurę – hetmana"
}

function Squares() {
    game = new Chess('8/8/8/8/8/8/8/8 w - - 0 1')
    board.position(game.fen())

    header.innerHTML=" Opis szachownicy"
    topic.innerHTML = "Polem gry w szachy jest szachownica w kształcie kwadratu składającą się z 64 pól w dwóch kolorach. Białe i czarne pola ułożone są naprzemiennie w 8 rzędach (opisanych cyframi od 1 do 8) i kolumnach (opisanych literami od „a” do „h”)"
}

function Take() {
    game = new Chess('3qQ3/k5n1/8/2p2N2/1P3b2/4B3/K7/6Rr w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Zbijanie bierek"
    topic.innerHTML ="Bicie bierki wystepuje kiedy dana bierka może wykonać mormalny ruch na dane pole ale jest tam figura przeciwnika wtedy dana figura zastępuje miejsce figury zbitej a zbita figura jest usuwana z szachownicy wyjątkiem są piony które biją na ukos a nie do przodu"
}

function Check() {
    game = new Chess('8/3k4/8/7b/5N2/8/4KP2/8 w - - 0 1')
    board.position(game.fen())

    header.innerHTML="Szach"
    topic.innerHTML = "Jeżeli jeden z graczy zaatakuje króla przeciwnika – mówimy „szach!” Oznacza to, że monarcha jest w niebezpieczeństwie. Gracz, który został zaszachowany jest zobowiązany niezwłocznie do obrony swojego dowódcy. Może dokonać tego na 3 sposoby: zbicie bierki atakującej króla ,zasłonięcie się inną bierką z drużyny, bądź uciec królem na nie szachowane pole ."
}

function CheckMate() {
    game = new Chess('7k/1Q6/5K2/8/8/8/8/8 w - - 0 1')
    board.position(game.fen())

    header.innerHTML="Szach mat"
    topic.innerHTML = "Sytuację, w której jeden z zawodników dał szacha, a jego przeciwnik  nie może uratować króla w kolejnym posunięciu nazywamy matem. Mat kończy partię (w tym przykładze Hg7#)"
}

function Stalemate() {
    game = new Chess('7k/8/6Q1/4K3/8/8/8/8 b - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Pat"
    topic.innerHTML ="Pat powstaje tylko wtedy, gdy żadna bierka (nie tylko król) nie może wykonać prawidłowego ruchu. Im mniej bierek na szachownicy, tym większe jest więc prawdopodobieństwo doprowadzenia do sytuacji patowej"
}

function Material() {
    game = new Chess('7k/8/5K2/8/8/8/8/8 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Brak bierek do mata"
    topic.innerHTML="Remis w tym przypadku nastepuję kiedy na szachownicy jest tyle że nie jest możliwym utworzenie pozycji w której którys z królów zostaje zamatowany najczęsciej taka sytuacja to król kontra król lub król i skoczek kontar król (uwaga! król kontra król i pionek to nie automatyczny remis gdyż pionek może może byc awansowany na Hetman bądź wieże) "
}

function Repetition() {
    game = new Chess('8/8/7k/7p/7P/7K/8/8 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Powtórzenie pozycji"
    topic.innerHTML = "Remis następuje, gdy po raz trzeci na szachownicy pojawi się całkowicie identyczna pozycja"
}

function FiftyMove() {
    game = new Chess('8/8/7k/7p/7P/7K/8/8 w - - 0 1')
    board.position(game.fen())

    header.innerHTML = "Zasada 50 ruchów"
    topic.innerHTML = "Partia kończy się remisem, jeśli obie strony przez ostatnie 50 ruchów nie wykonały żadnego posunięcia pionkiem i nie nastąpiło żadne zbicie"
}

var board = null
var game = new Chess()
var whiteSquareGrey = '#a9a9a9'
var blackSquareGrey = '#696969'

function removeGreySquares() {
    $('#myBoard .square-55d63').css('background', '')
}

function greySquare(square) {
    var $square = $('#myBoard .square-' + square)

    var background = whiteSquareGrey
    if ($square.hasClass('black-3c85d')) {
        background = blackSquareGrey
    }

    $square.css('background', background)
}

function onDragStart(source, piece) {
    // do not pick up pieces if the game is over
    if (game.game_over()) return false

    // or if it's not that side's turn
    if ((game.turn() === 'w' && piece.search(/^b/) !== -1) ||
        (game.turn() === 'b' && piece.search(/^w/) !== -1)) {
        return false
    }
}

function onDrop(source, target) {
    removeGreySquares()

    // see if the move is legal
    var move = game.move({
        from: source,
        to: target,
        promotion: 'q' // NOTE: always promote to a queen for example simplicity
    })

    // illegal move
    if (move === null) return 'snapback'
}

function onMouseoverSquare(square, piece) {
    // get list of possible moves for this square
    var moves = game.moves({
        square: square,
        verbose: true
    })

    // exit if there are no moves available for this square
    if (moves.length === 0) return

    // highlight the square they moused over
    greySquare(square)

    // highlight the possible squares for this piece
    for (var i = 0; i < moves.length; i++) {
        greySquare(moves[i].to)
    }
}

function onMouseoutSquare(square, piece) {
    removeGreySquares()
}

function onSnapEnd() {
    board.position(game.fen())
}

var config = {
    pieceTheme: '/img/chesspieces/wikipedia/{piece}.png',
    draggable: true,
    position: '8/8/8/8/8/8/8/8',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onMouseoutSquare: onMouseoutSquare,
    onMouseoverSquare: onMouseoverSquare,
    onSnapEnd: onSnapEnd
}
board = Chessboard('myBoard', config)






