using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioHelm;
using System;

public class QuickTuple<T1, T2> {
	public T1 first { get; private set;}
	public T2 last { get; private set;}
	public QuickTuple(T1 f, T2 l) {
		first = f; 
		last = l;
	}
}
public class AbstractMarkovMusic : object {
	Dictionary<string, List<int>> training_data;
	public AbstractMarkovMusic() {
		training_data = new Dictionary<string, List<int>> ();
		training_data.Add ("70_70", new List<int>(new int[] {70,71,72}));
		training_data.Add ("70_71", new List<int>(new int[] {70,72}));
		training_data.Add ("71_70", new List<int>(new int[] {72}));
		training_data.Add ("70_72", new List<int>(new int[] {70, 71}));
		training_data.Add ("71_72", new List<int>(new int[] {70, 71}));


	}
	public AbstractMarkovMusic(string file_data) {
		training_data = new Dictionary<string, List<int>> ();
		// TODO: Parsing the file data here.
	}
public QuickTuple<int, float> getNextNote(int second_last, int last) {
		return new QuickTuple<int, float> (getNextNotePitch (second_last, last), getNextNoteLength (second_last, last));
	}

	public int getNextNotePitch(int second_last, int last) {
		string key = second_last + "_" + last;
		Debug.Log ("Key is...");
		if (training_data.ContainsKey (key)) {
			Debug.Log ("Key in dict");
			List<int> result = training_data [key];
			if (result.Count == 0) {
				return defaultNotePitch (second_last, last);
			}
			Debug.Log ("result.count > 0");
			return result [UnityEngine.Random.Range (0, result.Count)];
		} else {
			Debug.Log ("Key not in dict");
			return defaultNotePitch (second_last, last);
		}
	}

	public virtual int defaultNotePitch(int second_last, int last) {
		return 72;
	}

	public virtual float getNextNoteLength(int second_last, int last) {
		return 8;
	}

	public void addNextBeats(int beats, HelmSequencer sequencer) {
		QuickTuple<Note, Note> last_two;
		last_two = getLastTwoNotes (sequencer);
		float start;
		if (last_two.last != null) { 
			start = last_two.last.end;
		} else {
			start = 0f;
		}
		int second_last, last;
		float currentEnd = start;
		
		while (currentEnd < start + beats) {
			if (last_two.first != null) { 
				second_last = last_two.first.note;
			} else {
				second_last = 0;
			}
			if (last_two.last != null) {
				last = last_two.last.note;
			} else {
				last = 0;
			}
			QuickTuple<int, float> nextNote = getNextNote (second_last, last);
			Debug.Log ("next note is... (" + nextNote.first + "," + nextNote.last+")");
			Debug.Log ("Current end is..."+currentEnd);
			sequencer.AddNote (nextNote.first, currentEnd, currentEnd + nextNote.last);
			last_two = getLastTwoNotes (sequencer);
			currentEnd = last_two.last.end;
		}
	}

	public QuickTuple<Note, Note> getLastTwoNotes(HelmSequencer sequencer) {
		List<Note> notes = sequencer.GetAllNoteOnsInRange (0, sequencer.length);
		Debug.Log ("Notes: " + notes);
		Debug.Log ("There are " + notes.Count + " Notes here");
		if (notes.Count == 0) { 
			return new QuickTuple<Note, Note> (null, null);
		} else if (notes.Count == 1) {
			return new QuickTuple<Note, Note> (null, notes [0]);
		} else {
			return new QuickTuple<Note, Note> (notes [notes.Count - 2], notes [notes.Count - 1]);
		}
	}
}
