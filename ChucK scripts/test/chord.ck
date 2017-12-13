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