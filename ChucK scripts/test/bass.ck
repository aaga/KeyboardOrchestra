public class Bass {
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