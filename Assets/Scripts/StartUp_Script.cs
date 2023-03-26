using System;
using System.IO;
using UnityEngine;

public class StartUp_Script : MonoBehaviour
{
    private string candidatesPath = "/Users/ibnzwanckaert/Projects/Matching/candidates.json";
    private string jobsPath = "/Users/ibnzwanckaert/Projects/Matching/jobs.json";

    void Start()
    {
        Controller c = GetComponent<Controller>();

        string candidatesTxt = File.ReadAllText(candidatesPath);
        CandidateWrapper candidateWrapper = JsonUtility.FromJson<CandidateWrapper>(candidatesTxt);

        string jobsTxt = File.ReadAllText(jobsPath);
        JobWrapper jobWrapper = JsonUtility.FromJson<JobWrapper>(jobsTxt);

        c.Initialize(jobWrapper.infos, candidateWrapper.infos);
    }
}

[Serializable]
struct CandidateWrapper
{
    public CandidateData[] infos;
}

[Serializable]
public struct JobWrapper
{
    public JobData[] infos;
}