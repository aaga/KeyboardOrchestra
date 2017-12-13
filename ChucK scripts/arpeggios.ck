SqrOsc foo;

ADSR e;
LPF lpf;
NRev r;

foo => e => lpf => r => dac;

// oscillator
20 => foo.freq;

// adsr
5::ms => e.attackTime;
5::ms => e.decayTime;
.5 => e.sustainLevel;
0::ms => e.releaseTime;

// filter
2000 => lpf.freq;
1 => lpf.Q;

// reverb
0 => r.mix;

40 => int millis;
60 => int start;
1 => int major; // 0 for minor

while( true )
{
    Std.mtof(start) => foo.freq;
    1 => e.keyOn;
    millis::ms => now;
    0 => e.keyOn;
    
    Std.mtof(start + 3 + major) => foo.freq;
    1 => e.keyOn;
    millis::ms => now;
    0 => e.keyOn;
    
    Std.mtof(start + 7) => foo.freq;
    1 => e.keyOn;
    millis::ms => now;
    0 => e.keyOn;
}