// launch with r.ck

// name
"localhost" => string hostname;
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

// infinite time loop
while( true )
{
    1::second => now;
}