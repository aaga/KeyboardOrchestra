DrumSet drm;
drm.connect( dac );

Bass bass;
bass.connect( dac );

[ 41, 41, 44, 46] @=> int bline[];
0 => int pos;
0 => int count;

while ( true ) {
    drm.hh();
    if ( count % 2 == 0 ) { drm.bd(); }
    if ( count % 4 == 2 ) { drm.sn(); }
    
    if ( count % 2 == 0 ) { spork ~ bass.bass( bline[ pos % 4 ]); }
    if ( count % 2 == 1 ) { spork ~ bass.bass( 12 + bline[ pos % 4 ]); }
    
    
    1 + count => count;
    if ( count % 4 == 0 ) { 1 + pos => pos; }
    250::ms => now;
}