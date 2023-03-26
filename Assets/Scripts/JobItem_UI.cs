using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class JobItem_UI
{
    private Color good_Color = new Color(0.4745098f, 0.7843137f, 0.6039216f);
    private Color medium_Color = new Color(0.7843137f, 0.725959f, 0.4745098f);
    private Color bad_Color = new Color(0.7843137f, 0.541252f, 0.4745098f);
    private Color worst_Color = new Color(0.7843137f, 0.4745098f, 0.5385448f);

    private Label Job_Name;
    private Label Job_Descr;
    private Label Job_Sector;
    private Label Job_City;
    private Label Job_Statutes;
    private Label Job_MinAge;
    private Label Job_Education;
    private Label Job_Long;
    private Label Job_Lat;
    private Label Job_Distance;
    private Label Job_MatchScore;
    private Button Job_ShowInterest;
    private VisualElement Job_Image;
    private Controller m_controller;
    private VisualElement Job_Background;
    private ExtendedJob m_Job;

    public void SetVisualElement(VisualElement element)
    {
        Job_Image = element.Q<VisualElement>("Picture");
        Job_Name = element.Q<Label>("Job_Name");
        Job_Descr = element.Q<Label>("Job_Descr");
        Job_Sector = element.Q<Label>("Job_Sector");
        Job_City = element.Q<Label>("Job_City");
        Job_Statutes = element.Q<Label>("Job_Statutes");
        Job_MinAge = element.Q<Label>("Job_MinAge");
        Job_Education = element.Q<Label>("Job_MinEducation");
        Job_Lat = element.Q<Label>("Job_Lat");
        Job_Long = element.Q<Label>("Job_Long");
        Job_MatchScore = element.Q<Label>("Job_Matcher");
        Job_Distance = element.Q<Label>("Job_Distance");
        Job_Background = element.Q<VisualElement>("BG_Picture");
        Job_ShowInterest = element.Q<Button>("Accept");
        Job_ShowInterest.clicked += OnAcceptJob;
    }

    public void OnAcceptJob()
    {
        m_controller.ShowInterestOnJob(m_Job);
    }

    public void SetData(ExtendedJob jobItem, Sprite image, Controller controller)
    {
        m_controller = controller;
        m_Job = jobItem;

        Job_Image.style.backgroundImage = new StyleBackground(image);
        Job_Name.text = jobItem.job.jobName;
        Job_Descr.text = jobItem.job.description;
        Job_Sector.text = System.Enum.GetName(typeof(Sector),jobItem.job.sector);
        Job_City.text = jobItem.job.city;
        string statutes = "";
        foreach (var stat in jobItem.job.statutes)
        {
            statutes += System.Enum.GetName(typeof(Statute), stat) + ", ";
        }
        Job_Statutes.text = statutes.Substring(0, statutes.Length - 2);
        Job_MinAge.text = jobItem.job.minAge.ToString();
        Job_Education.text = jobItem.job.minEducationLvl.ToString();
        Job_Lat.text = Math.Round(jobItem.job.latiture, 5).ToString();
        Job_Long.text = Math.Round(jobItem.job.longitude, 5).ToString();
        Job_Distance.text = Math.Round(jobItem.distance, 5).ToString();
        Job_MatchScore.text = Mathf.Ceil(jobItem.matcherScore).ToString();

        Job_Background.style.backgroundColor = getColorByQuality(jobItem.jobQuality);
    }

    private Color getColorByQuality(JobQuality quality)
    {
        switch(quality)
        {
            case JobQuality.good: return good_Color;
            case JobQuality.medium: return medium_Color;
            case JobQuality.bad: return bad_Color;
            case JobQuality.worst: return worst_Color;
            default: return Color.white;
        }
    }
}
