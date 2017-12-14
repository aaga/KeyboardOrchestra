class Chord {
    
    5 => int size;
    
    BlitSquare osc[5];
    ADSR adsr[5];
    
    Gain g => dac;
    
    50::ms => dur attack;
    25::ms => dur decay;
    0.5 => float sustain;
    25::ms => dur release;
    
    0.3 => g.gain;
    
    for (0 => int i; i < size; i++) {
        osc[i] => adsr[i];
        adsr[i] => g;
        adsr[i].set(attack, decay, sustain, release);
    }
    
    public void play(int num, int note) {
        Std.mtof(note) => osc[num].freq;
        1 => adsr[num].keyOn;
    }
    
    public void softOff() {
        for (0 => int i; i < size; i++) {
            1 => adsr[i].keyOff;
        }
    }
    
    public void hardOff() {
        for (0 => int i; i < size; i++) {
            0::ms => adsr[i].releaseTime;
        }
        softOff();
        for (0 => int i; i < size; i++) {
            release => adsr[i].releaseTime;
        }
    }
    
}

class DrumSet {
    // define hihat
    Shakers hhs => JCRev r;
    .025 => r.mix;
    Std.mtof( 76 ) => hhs.freq;
    
    // Define Bassdrum
    SinOsc s => ADSR bda;
    80 => s.freq;
    (0::ms, 10::ms, 0.0, 0::ms ) => bda.set;
    
    // define snare drum
    Noise n => ADSR sna => Gain g;
    0.15 => g.gain;
    (0::ms, 25::ms, 0.0, 0::ms) => sna.set;
    
    
    public void connect( UGen ugen ) {
        r => ugen;
        bda => ugen;
        g => ugen;
    }
    
    public void hh() {
        1 => hhs.noteOn;
    }
    
    public void bd() {
        1 => bda.keyOn;
    }
    
    public void sn() {
        1 => sna.keyOn;
    }
}

class Bass {
    // BASS
    SawOsc sb => LPF filt => ADSR a => Gain g2;
    440 => filt.freq;
    0.3 => filt.Q;
    0.0 => g2.gain;
    (10::ms, 45::ms, 0.5, 40::ms) => a.set; // Set ADSR envelope
    
    public void connect( UGen u ) {
        g2 => u;
    }
    
    public void bass( int tone ) {
        Std.mtof( tone ) =>  sb.freq;
        0.3 => g2.gain;
        1 => a.keyOn;
        125::ms => now;
        1=> a.keyOff;
    }
}

class BassDrumLoop {
    
	Gain bdlGain => dac;
	bdlGain.gain(1);
    DrumSet drm;
    drm.connect( bdlGain );
    
    Bass bass;
    bass.connect( bdlGain );
    
    [ 41, 41, 44, 46] @=> int bline[];
    0 => int pos;
    0 => int count;
    
    250::ms => dur length;
    
    public void setLength(dur l) {
        l => length;
    }
    
    public void reset() {
        0 => pos;
        0 => count;
    }
    
    public void setKey(int key) {
        [ key, key, key + 3, key + 5] @=> bline;
    }
    
    public void setGain(float gainToSet) {
        bdlGain.gain(gainToSet);
    }
    
    public void stop() {
        setGain(0);
    }
    
    public void play() {
        while ( true ) {
            drm.hh();
            if ( count % 2 == 0 ) { drm.bd(); }
            if ( count % 4 == 2 ) { drm.sn(); }
            
            if ( count % 2 == 0 ) { spork ~ bass.bass( bline[ pos % 4 ]); }
            if ( count % 2 == 1 ) { spork ~ bass.bass( 12 + bline[ pos % 4 ]); }
            
            
            1 + count => count;
            if ( count % 4 == 0 ) { 1 + pos => pos; }
            length => now;
        }
    }
    
}
BassDrumLoop bdl;
Chord chord;