using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Controller : MonoBehaviour
{
    private ExtendedCandidate[] m_candidates = new ExtendedCandidate[0];
    private JobData[] m_jobs = new JobData[0];
    private ExtendedCandidate m_activeCandidate;
    private const float m_JobUpInterestValue = 10f;
    private const float m_JobDownInterestValue = 2.5f;
    private const int m_CandidateStartPrefrence = 50;
    private const int m_CandidateMinPrefrence = 0;
    private const int m_CandidateMaxPrefrence = 100;

    private Job_UI job_UI;
    private Candidates_UI candidate_UI;
    private Matcher matcher;

    public void Initialize(JobData[] jobs, CandidateData[] candidates)
    {
        job_UI = GetComponent<Job_UI>();
        candidate_UI = GetComponent<Candidates_UI>();
        matcher = new Matcher(this);

        m_candidates = candidates.Select(candidate => new ExtendedCandidate()
        {
            candidate = candidate,
            realSectorPrefrence = new SectorPrefrence[]
            {
                new SectorPrefrence()
                {
                    sector = Sector.Administrative,
                    prefrence = m_CandidateStartPrefrence,
                },
                new SectorPrefrence()
                {
                    sector = Sector.CustomerService,
                    prefrence = m_CandidateStartPrefrence,
                },
                new SectorPrefrence()
                {
                    sector = Sector.Flex,
                    prefrence = m_CandidateStartPrefrence,
                },
                new SectorPrefrence()
                {
                    sector = Sector.Food,
                    prefrence = m_CandidateStartPrefrence,
                },
                new SectorPrefrence()
                {
                    sector = Sector.Industry,
                    prefrence = m_CandidateStartPrefrence,
                },
                new SectorPrefrence()
                {
                    sector = Sector.Trade,
                    prefrence = m_CandidateStartPrefrence,
                },

            },
        }).ToArray();
        m_jobs = jobs;
    }

    public ExtendedCandidate RequestActiveCandidate()
    {
        return m_activeCandidate;
    }

    public ExtendedCandidate RequestCandidate(bool newActive,int id = -1)
    {
        if (id >= m_candidates.Length) throw new System.Exception("Out of bounds: Candidate Id is to large");
        if (id == -1) id = (int)(Random.value * m_candidates.Length);
        if (newActive) m_activeCandidate = m_candidates[id];
        m_candidates[id].candidate.id = id;
        return m_candidates[id];
    }

    public JobData RequestJob(int id = -1)
    {
        if (id >= m_jobs.Length) throw new System.Exception("Out of bounds: Job Id is to large");
        if (id == -1) id = (int)(Random.value * m_jobs.Length);
        return m_jobs[id];
    }

    public JobData[] RequestAllJobs()
    {
        return m_jobs;
    }

    public ExtendedCandidate[] RequestAllCandidates()
    {
        return m_candidates;
    }

    public void ShowInterestOnJob(ExtendedJob job)
    {
        for (int i = 0; i < m_activeCandidate.realSectorPrefrence.Length; i++)
        {
            if (m_activeCandidate.realSectorPrefrence[i].sector == job.job.sector)
                m_activeCandidate.realSectorPrefrence[i].prefrence += m_JobUpInterestValue;
            else
                m_activeCandidate.realSectorPrefrence[i].prefrence -= m_JobDownInterestValue;

            m_activeCandidate.realSectorPrefrence[i].prefrence
                = Mathf.Clamp(m_activeCandidate.realSectorPrefrence[i].prefrence, m_CandidateMinPrefrence, m_CandidateMaxPrefrence);
        }
        candidate_UI.fillForm(m_activeCandidate);
        Rematch();
    }

    public void Rematch()
    {
        ExtendedJob[] jobs = matcher.MatchForCandidate(m_activeCandidate);
        job_UI.fillList(jobs);
    }
}
