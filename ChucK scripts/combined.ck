0 => external int messageToSend;
0 => external int messageReceived;
external Event sendMessage;
external Event notifier;

// address of other computer
"localhost" => string hostname;
// sending on port
6449 => int port;

// check command line
//if( me.args() ) me.arg(0) => hostname;
//if( me.args() > 1 ) me.arg(1) => Std.atoi => port;

// send object
OscSend xmit;

// aim the transmitter
xmit.setHost( hostname, port );

fun void sendIntMessage(int messageInt) {
    // start the message...
    // the type string 'i' expects an int
    xmit.startMsg( "/keyboardOrchestra/keyPressInfo", "i" );
    
    // a message is kicked as soon as it is complete 
    // - type string is satisfied and bundles are closed
    messageInt => xmit.addInt;
}

// create our OSC receiver
OscRecv recv;
// use port 6439 (or whatever)
6439 => recv.port;
// start listening (launch thread)
recv.listen();

// create an address in the receiver, store in new variable
recv.event( "/keyboardOrchestra/keyPressInfo, i" ) @=> OscEvent @ oe;

fun void receiveIntMessage () {
    // infinite event loop
    while( true )
    {
        // wait for event to arrive
        oe => now;
        
        // grab the next message from the queue. 
        while( oe.nextMsg() )
        {        
            // getInt fetches the expected int (as indicated by "i")
            oe.getInt() => messageReceived;
            notifier.broadcast();
            
            // print
            <<< "got (via OSC):", messageReceived >>>;
        }
    }
}

spork ~ receiveIntMessage();

// infinite time loop
while( true )
{
    sendMessage => now;
    spork ~ sendIntMessage(messageToSend);
}