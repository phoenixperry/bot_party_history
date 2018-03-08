using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMarkovMusic : AbstractMarkovMusic {

	public override int defaultNotePitch(int second_last, int last) {
		return 70;
	}
}
