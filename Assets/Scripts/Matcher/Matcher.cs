using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Matcher
{
    private Controller m_controller;

    // how much does it matter?
    private const float m_prefrenceScalar = 1f;
    private const float m_distanceScalar = 1f;
    private const float m_educationScalar = 0.5f;
    private const float m_hiddenPrefrenceScalar = 1f;
    private float m_amountOfScalars;

    public Matcher(Controller controller)
    {
        m_controller = controller;
        m_amountOfScalars = m_prefrenceScalar + m_distanceScalar + m_educationScalar + m_hiddenPrefrenceScalar;
    }

    public ExtendedJob[] MatchForCandidate(ExtendedCandidate candidate)
    {
        System.Random rnd = new System.Random();

        // Find jobs with params
        JobData[] availableJobs = m_controller.RequestAllJobs()
            .Where(job => candidate.candidate.age >= job.minAge &&
                   candidate.candidate.educationLvl >= job.minEducationLvl &&
                   job.statutes.Any(s => candidate.candidate.statute == s))
            .ToArray();

        // Match the jobs
        List<ExtendedJob> jobs = new List<ExtendedJob>();
        foreach (var availableJob in availableJobs)
        {
            ExtendedJob extendedJob = addMatcherScores(availableJob, candidate);
            if(extendedJob.passedDynamicCheck) jobs.Add(extendedJob);
        }

        // Order the jobs
        return OrderJobs(jobs.ToArray());
    }

    private ExtendedJob addMatcherScores(JobData job, ExtendedCandidate candidate)
    {
        ExtendedJob extendedJob = new ExtendedJob()
        {
            job = job,
            matcherScore = 0,
            passedDynamicCheck = true,
        };

        // scores are always expressed as (0 - 100) * Scalar
        extendedJob.matcherScore += sectorPrefrenceScore(candidate, ref extendedJob) * m_prefrenceScalar;
        extendedJob.matcherScore += hiddenSectorPrefrenceScore(candidate, ref extendedJob) * m_hiddenPrefrenceScalar;
        extendedJob.matcherScore += distanceScore(candidate, ref extendedJob) * m_distanceScalar;
        extendedJob.matcherScore += educationScore(candidate, ref extendedJob) * m_educationScalar;
        extendedJob.matcherScore /= m_amountOfScalars;

        if (extendedJob.matcherScore > 100) extendedJob.matcherScore = 100;
        if (extendedJob.matcherScore < 0) extendedJob.matcherScore = 0;
        return extendedJob;
    }

    private ExtendedJob[] OrderJobs(ExtendedJob[] jobs)
    {
        jobs = jobs.OrderByDescending(j => j.matcherScore).ToArray();

        int goodMaxIndex = jobs.Length / 4;
        int mediumMaxIndex = jobs.Length / 4 * 2;
        int badMaxIndex = jobs.Length / 4 * 3;

        Queue<ExtendedJob> goodJobs = new Queue<ExtendedJob>(jobs[0..goodMaxIndex]);
        Queue<ExtendedJob> mediumJobs = new Queue<ExtendedJob>(jobs[goodMaxIndex..mediumMaxIndex]);
        Queue<ExtendedJob> badJobs = new Queue<ExtendedJob>(jobs[mediumMaxIndex..badMaxIndex]);
        Queue<ExtendedJob> worstJobs = new Queue<ExtendedJob>(jobs[badMaxIndex..jobs.Length]);

        List<ExtendedJob> fixedJobs = new List<ExtendedJob>();
        while(fixedJobs.Count < 20 &&
            (goodJobs.Count > 0 || mediumJobs.Count > 0 || badJobs.Count > 0 || worstJobs.Count > 0))
        {
            if(fixedJobs.Count < 8 && goodJobs.Count > 0)
            {
                ExtendedJob job = goodJobs.Dequeue();
                job.jobQuality = JobQuality.good;
                fixedJobs.Add(job);
                continue;
            }
            if (fixedJobs.Count < 13 && mediumJobs.Count > 0)
            {
                ExtendedJob job = mediumJobs.Dequeue();
                job.jobQuality = JobQuality.medium;
                fixedJobs.Add(job);
                continue;
            }
            if (fixedJobs.Count < 18 && badJobs.Count > 0)
            {
                ExtendedJob job = badJobs.Dequeue();
                job.jobQuality = JobQuality.bad;
                fixedJobs.Add(job);
                continue;
            }
            if (worstJobs.Count > 0)
            {
                ExtendedJob job = worstJobs.Dequeue();
                job.jobQuality = JobQuality.worst;
                fixedJobs.Add(job);
                continue;
            }
        }

        return fixedJobs.ToArray();
    }

    private float sectorPrefrenceScore(ExtendedCandidate candidate, ref ExtendedJob job)
    {
        ExtendedJob copy = job;
        SectorPrefrence sectorPrefrence = candidate.candidate.sectorPreferences.Where(j => j.sector == copy.job.sector).First();
        float prefrenceScore = sectorPrefrenceToScores(sectorPrefrence);
        return prefrenceScore;
    }

    private float sectorPrefrenceToScores(SectorPrefrence prefrence)
    {
        switch(prefrence.prefrence)
        {
            case 1: return -25;
            case 2: return 0;
            case 3: return 30;
            case 4: return 75;
            case 5: return 100;
            default: return 0;
        }
    }

    private float hiddenSectorPrefrenceScore(ExtendedCandidate candidate, ref ExtendedJob job)
    {
        ExtendedJob copy = job;
        SectorPrefrence sectorPrefrence = candidate.realSectorPrefrence.Where(sector => sector.sector == copy.job.sector).First();
        return sectorPrefrence.prefrence;
    }

    private float educationScore(ExtendedCandidate candidate, ref ExtendedJob job)
    {
        int knowledgeScore = candidate.candidate.educationLvl > candidate.candidate.yrsOfExperience ?
            candidate.candidate.educationLvl : candidate.candidate.yrsOfExperience;

        float educationScore = heavyScalar(job.job.minEducationLvl, knowledgeScore); //0-10 * 0-10
        return educationScore * 10;
    }

    private float heavyScalar(float v1,float v2)
    {
        return Mathf.Sqrt(v1 * v2);
    }

    private float distanceScore(ExtendedCandidate candidate, ref ExtendedJob job)
    {
        float distance = Distance(new Vector2(candidate.candidate.latiture, candidate.candidate.longitude), new Vector2(job.job.latiture, job.job.longitude));
        distance = (float) Math.Round(distance, 4);
        job.distance = distance;
        float distanceScore = distanceToScore(distance);

        if (distanceScore < 0) job.passedDynamicCheck = false;
        if(job.job.city == candidate.candidate.city)
        {
            distanceScore = 100;
            job.distance = 0;
        }
        return distanceScore;
    }

    private float distanceToScore(float distance)
    {
        if (distance < 0.5f) return 100;
        if (distance < 1f) return 50;
        if (distance < 1.5f) return 25;
        if (distance < 2f) return 0;
        return -1;
    }

    private float Distance(Vector2 candCoords, Vector2 jobCoords)
    {
        float originX = candCoords.x - jobCoords.x;
        float originY = candCoords.y - jobCoords.y;
        return Mathf.Sqrt(originX * originX + originY * originY);
    }
}
