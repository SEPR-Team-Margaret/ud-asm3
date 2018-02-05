using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeutralAI : MonoBehaviour {
    private int playerID = 3;
    private Game game;

    void Start() {
        game = GameObject.Find("EventManager").GetComponent<Game>();
    }

    int SumOfAdjacentEnemies(Section section){
        return 0;
    }

    PairSections DeltaAdjacents(Section section, bool friendly = true){
        // Return a pair of sections based on the local max delta of unit magnitudes between a section and it neighbours

        int curMaxDelta = 0;
        Section sectionHi = null;
        Section sectionLo = null;

        foreach (var adjSection in section.adjacentSectors)
        {
            if (friendly && adjSection.GetOwner() == playerID && section.GetOwner() == playerID)
            {
                if (Mathf.Abs(adjSection.GetUnits() - section.GetUnits()) > curMaxDelta)
                {
                    if (adjSection.GetUnits() > section.GetUnits())
                    {
                        sectionHi = adjSection;
                        sectionLo = section;
                        curMaxDelta = Mathf.Abs(adjSection.GetUnits() - section.GetUnits());
                    }
                    else
                    {
                        sectionHi = section;
                        sectionLo = adjSection;
                        curMaxDelta = Mathf.Abs(adjSection.GetUnits() - section.GetUnits());
                    }
                }
            }
            else if (!friendly && adjSection.GetOwner() != playerID && section.GetOwner() == playerID)
            {
                if (adjSection.GetUnits() - section.GetUnits() > curMaxDelta)
                {
                    sectionHi = adjSection;
                    sectionLo = section;
                    curMaxDelta = adjSection.GetUnits() - section.GetUnits();
                }
            }
        }
        if (sectionHi != null && sectionLo != null && curMaxDelta != 0)
        {
            PairSections returnPairSections = new PairSections();
            returnPairSections.sectionHi = sectionHi;
            returnPairSections.sectionLo = sectionLo;
            returnPairSections.delta = curMaxDelta;

            return returnPairSections;
        }
        return null;
    }

    PairSections DeltaAllFriendlyAdjacents(){
    // Return the pair of sections based on the global max delta between two sections owned by the neutral AI
        Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
        PairSections maxPair = null;
        foreach (var sect in sections){
            if (sect.GetOwner() == playerID) {
                PairSections curPair = DeltaAdjacents(sect);
                if (curPair != null)
                {
                    if (maxPair == null)
                    {
                        maxPair = curPair;
                    }
                    else if (curPair.delta > maxPair.delta)
                    {
                        maxPair = curPair;
                    }
                }
            }
            
        }
        if (maxPair != null){
            return maxPair;
        }
        return null;
    }

    PairSections[] DeltaAllThreats(){
        // Iterate over friendly sectors checking for adjacent enemy sectors
        // Return the top 10 worst offenders 
        Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
        var pairs = new List<PairSections>();
        foreach (var sect in sections)
        {
            PairSections curAdj = DeltaAdjacents(sect, false);
            if (curAdj != null){
                pairs.Add(curAdj);
            }
            
        }
        pairs.Sort();

        
        int maxBound = 10;
        if (pairs.Count < 10){
            maxBound = pairs.Count;
        }
        PairSections[] returnPairs = new PairSections[maxBound];
        for (var i = 0; i < maxBound || maxBound == 0; i++){
            returnPairs[i] = pairs[i];
        }
        if (returnPairs.Length != 0){
            return returnPairs;
        }
        return null;
    }

    OhBabyATriple FindFirstValidReinforce(PairSections[] sections)
    {
        // Find the first pair of sectors where one can reinforce another to mitigate a threat

       // sections: array of 10 sections which have high threat level
        foreach (var sect in sections){
            // It is implied by previous logic that sectionLo is the friendly sector
            // It is implied by previous logic that sectionHi is the enemy sector
            PairSections maxAvailableReinforce = DeltaAdjacents(sect.sectionLo);
            if (maxAvailableReinforce != null){
                if (maxAvailableReinforce.delta + maxAvailableReinforce.sectionLo.GetUnits() > sect.sectionHi.GetUnits())
                {
                    int toMove = (sect.sectionHi.GetUnits() - sect.sectionLo.GetUnits()) + 1;
                    //If break; panic first; check here second

                    OhBabyATriple returnTriple = new OhBabyATriple();
                    returnTriple.source = maxAvailableReinforce.sectionHi;
                    returnTriple.destination = sect.sectionLo;
                    /*
                    returnTriple.enemy = sect.sectionHi;
                    */
                    returnTriple.toMove = toMove;
                    return returnTriple;
                }
            }
        }
        return null;
    }


    public void DecideMove(){
        // Attempt Reinforce vuln sectors
        Debug.Log("#PREINFORCE");
        Debug.Log("PreCalc Threats");
        PairSections[] threatPairs = DeltaAllThreats();

        /* 10 PairSections
         * Where Hi is enemy and Lo is friendly
         * Sorted by threat
         * Non-Null for every element
         */
        // TEST BEGIN
        Debug.Assert(threatPairs.Length > 0 && threatPairs.Length <= 10);
        Debug.Assert(threatPairs != null);
        foreach (var pair in threatPairs)
        {
            Debug.Assert(pair != null);
            Debug.Assert(pair.sectionHi != null);
            Debug.Assert(pair.sectionLo != null);
            Debug.Assert(pair.sectionHi.GetOwner() != 3);
            Debug.Assert(pair.sectionLo.GetOwner() == 3);
        }
        // TEST END
        Debug.Log("PreCalc Friendlies");
        PairSections maxDeltaPairSections = DeltaAllFriendlyAdjacents();
        /* Delta > 0
         * Non Null
         * Hi friendly
         * Lo friendly
         * 
         */
        //  TEST BEGIN
        Debug.Assert(maxDeltaPairSections.delta > 0);
        Debug.Assert(maxDeltaPairSections != null);
        Debug.Assert(maxDeltaPairSections.sectionHi != null);
        Debug.Assert(maxDeltaPairSections.sectionLo != null);
        Debug.Assert(maxDeltaPairSections.sectionHi.GetOwner() == 3);
        Debug.Assert(maxDeltaPairSections.sectionLo.GetOwner() == 3);
        // TEST END
        bool moveMade = false;

        if (threatPairs != null && !moveMade) {
            OhBabyATriple firstValidTriple = FindFirstValidReinforce(threatPairs);
            if (firstValidTriple != null) {
                DoMove(firstValidTriple.source, firstValidTriple.destination, firstValidTriple.toMove);
                moveMade = true;
                Debug.Log("REINFORCE");
            }
        }
        if (maxDeltaPairSections != null && !moveMade) {
            // Balance friendly sectors
            if (maxDeltaPairSections.delta > 2) {
                int unitsToMove = Mathf.FloorToInt(maxDeltaPairSections.delta / 2);
                DoMove(maxDeltaPairSections.sectionHi, maxDeltaPairSections.sectionLo, unitsToMove);
                moveMade = true;
                Debug.Log("BALANCE");
            }
        }
        if (!moveMade){
            // Else, end turn
            Debug.Log("ELSE END");
            moveMade = true;
        }
        game.NextTurn();
    }

    void DoMove(Section source, Section destination, int toMove){
        source.SetUnits(source.GetUnits() - toMove);
        destination.SetUnits(destination.GetUnits() + toMove);
    }
}

class PairSections : IComparable<PairSections>
{
    public Section sectionHi;
    public Section sectionLo;
    public int delta = 0;

    int IComparable<PairSections>.CompareTo(PairSections obj2){
        if (delta > obj2.delta){
            return 1;
        } else if (obj2.delta > delta){
            return -1;
        } else {
            return 0;
        }
    }
}

class OhBabyATriple
{
    //Mom get the camera
    public Section source;
    public Section destination;
    /*
    public Section enemy;
    */
    public int toMove;
}