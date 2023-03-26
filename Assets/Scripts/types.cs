using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ExtendedCandidate
{
    public CandidateData candidate;
    public SectorPrefrence[] realSectorPrefrence;
}

public struct ExtendedJob
{
    public JobData job;
    public float matcherScore;
    public float distance;
    public bool passedDynamicCheck;
    public JobQuality jobQuality;
}

public enum JobQuality
{
    good,
    medium,
    bad,
    worst
}

[Serializable]
public struct SectorPrefrence
{
    public Sector sector;
    public float prefrence;
}

[Serializable]
public struct CandidateData
{
    public string name;
    public int age;
    public Statute statute;
    public SectorPrefrence[] sectorPreferences;
    public int rating;
    public int yrsOfExperience;
    public float latiture;
    public float longitude;
    public string city;
    public int educationLvl;
    public int id;
}

[Serializable]
public struct JobData
{
    public string jobName;
    public string description;
    public Statute[] statutes;
    public int minAge;
    public Sector sector;
    public int minEducationLvl;
    public float latiture;
    public float longitude;
    public string city;
}

[Serializable]
public enum Sector
{
    Administrative = 0,
    CustomerService = 1,
    Flex = 2,
    Food = 3,
    Industry = 4,
    Trade = 5,
}

[Serializable]
public enum Statute
{
    Student,
    Normal,
    Flex
}
