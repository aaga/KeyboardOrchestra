public class DrumSet {
    // define hihat
    Shakers hhs => JCRev r;
    .025 => r.mix;
    Std.mtof( 76 ) => hhs.freq;
    
    // Define Bassdrum
    SinOsc s => ADSR bda;
    80 => s.freq;
    (0::ms, 10::ms, 0.0, 0::ms ) => bda.set;
    
    // define snare drum
    Noise n => ADSR sna => Gain g => dac;
    0.3 => g.gain;
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