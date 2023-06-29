# Random Avatar Animation e State Pattern

### Introduzione
Nel dominio dello spazio virtuale, che oggi chiamiamo, ad esempio, Metaverso, vive un concetto caratteristico: quello di Avatar.

Un avatar è una rappresentazione digitale di un essere vivente in uno spazio virtuale.

Ovviamente gli avatar sono animati e le loro animazioni sono gestite, ad esempio in Unity, con macchine a stati finiti implementate in un Animator Controller.

Oggi esistono molte tipologie di animazione e, per farsene un'idea, basta andare  a consultare il sito https://www.mixamo.com/ dove è possibile scaricarle gratuitamente per poi gestirle come stati autonomi, con relative transizioni, all'interno dell'Animator Controller in Unity.

Gli stati di animazione, sono stati a "grana fine", relativi appunto all'animazione dell'avatar. Possiamo, ad esempio, avere gli stati Start Walking, Stop Walking, Walking ecc. e, giocando con le transizioni di stato, combinare questi stati come vogliamo.

L'avatar, però, è un concetto di dominio, e non sempre è possibile associare con una relazione 1:1 lo stato dell'avatar con uno stato della sua animazione. Ad esempio, può essere per nulla rilevante avere uno stato dell'avatar come Start Walking o Stop Walking ma potrebbe essere invece eseere rilevante avere uno stato dell'avatar semplicemente Walking e, in quello stato, avere tutte le possibili combinazioni di animazioni di tipo Start Walking, Stop Walking, Walking.

Se devo controllare che il mio avatar non vada a sbattere contro un muro quello che importa è che lo faccia mentre l'avatar è nello stato di dominio Walking, poco importa se sta iniziando o finendo di camminare, altrimenti rischierei di dover fare una serie interminabile di if-else sparsi nel codice in tutti gli stati di animazione rilevanti per quel controllo.

A questo proposito, quindi, ho provato a disaccoppiare gli stati di dominio dagli stati di animazione utilizzando il buon vecchio State Pattern.

In questo semplice demo in cui un avatar di nome Bryce viene fatto muovere in maniera random, cercando il percorso più breve, gli stati di dominio sono gestiti tramite uno State Pattern che incapsula al suo interno i trigger necessari ai cambiamenti di stato dell'animazione da inviare all'Animator Controller di Unity.

Per questo demo ho utilizzato il character e le animazioni gratuite dal sito mixamo citato sopra, istruendo opportunamenet Unity a gestire direttamente le animazioni importate per lo spostamento della posizione nello spazio tridimensionale tramite Apply Root Motion.


[![](https://dl.dropboxusercontent.com/s/shzado7ockbxork/Avatar.png?dl=1)](https://dl.dropboxusercontent.com/s/tv5tz0eccsn1ge3/Avatar.mp4?dl=0)

