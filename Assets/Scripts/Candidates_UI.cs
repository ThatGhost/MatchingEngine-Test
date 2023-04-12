using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Candidates_UI : MonoBehaviour
{
    public GameObject JobView;
    public Sprite[] images;
    public UIDocument CandidateDoc;
    public UIDocument JobDoc;
    public Color positiveHidden;
    public Color negativeHidden;

    private Controller controller;
    private VisualElement Cand_Image;
    private Button randomizeButton;
    private Button hiddenButton;
    private Button jobsButton;
    private Button SaveSettingsButton;
    private Label Cand_name;
    private Label Cand_Age;
    private Label Cand_City;
    private Label Cand_Statute;
    private Label Cand_Rating;
    private Label Cand_YrsOfExp;
    private Label Cand_Education;
    private Label Cand_Longitude;
    private Label Cand_Latitude;
    private ProgressBar Cand_Admin;
    private ProgressBar Cand_CustomerService;
    private ProgressBar Cand_Flex;
    private ProgressBar Cand_Food;
    private ProgressBar Cand_Industry;
    private ProgressBar Cand_Trade;
    private Label Cand_Admin_Hidden;
    private Label Cand_CustomerService_Hidden;
    private Label Cand_Flex_Hidden;
    private Label Cand_Food_Hidden;
    private Label Cand_Industry_Hidden;
    private Label Cand_Trade_Hidden;


    private void Start()
    {
        controller = GetComponent<Controller>();
        var root = CandidateDoc.rootVisualElement;

        randomizeButton = root.Q<Button>("RandomButton");
        jobsButton = root.Q<Button>("ShowJobs");

        Cand_Image = root.Q<VisualElement>("CandImage");
        Cand_name = root.Q<Label>("Cand_Name");
        Cand_Age = root.Q<Label>("Cand_Age");

        Cand_Statute = root.Q<Label>("Cand_Statute");
        Cand_Rating = root.Q<Label>("Cand_Rating");
        Cand_YrsOfExp = root.Q<Label>("Cand_yrsOfExp");
        Cand_Education = root.Q<Label>("Cand_education");

        Cand_City = root.Q<Label>("Cand_City");
        Cand_Latitude = root.Q<Label>("Cand_Lat");
        Cand_Longitude = root.Q<Label>("Cand_Long");

        Cand_Admin = root.Q<ProgressBar>("Cand_Admin");
        Cand_CustomerService = root.Q<ProgressBar>("Cand_Service");
        Cand_Flex = root.Q<ProgressBar>("Cand_Flex");
        Cand_Food = root.Q<ProgressBar>("Cand_Food");
        Cand_Industry = root.Q<ProgressBar>("Cand_Industry");
        Cand_Trade = root.Q<ProgressBar>("Cand_Trade");

        Cand_Admin_Hidden = root.Q<Label>("Cand_Admin_hidden");
        Cand_CustomerService_Hidden = root.Q<Label>("Cand_Service_hidden");
        Cand_Flex_Hidden = root.Q<Label>("Cand_Flex_hidden");
        Cand_Food_Hidden = root.Q<Label>("Cand_Food_hidden");
        Cand_Industry_Hidden = root.Q<Label>("Cand_Industry_hidden");
        Cand_Trade_Hidden = root.Q < Label>("Cand_Trade_hidden");

        randomizeButton.clicked += OnRandomize;
        jobsButton.clicked += OnToJob;

        OnRandomize();
    }

    public void OnToJob()
    {
        CandidateDoc.rootVisualElement.style.display = DisplayStyle.None;
        JobDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }

    public void OnRandomize()
    {
        ExtendedCandidate candidateData = controller.RequestCandidate(true);
        controller.Rematch();
        fillForm(candidateData);
    }

    public void fillForm(ExtendedCandidate candidateData)
    {
        Cand_Image.style.backgroundImage = new StyleBackground(images[UnityEngine.Random.Range(0, images.Length)]);
        Cand_name.text = candidateData.candidate.name;
        Cand_Age.text = candidateData.candidate.age.ToString();
        Cand_City.text = candidateData.candidate.city;
        Cand_Statute.text = Enum.GetName(typeof(Statute), candidateData.candidate.statute);
        Cand_Rating.text = candidateData.candidate.rating.ToString();
        Cand_YrsOfExp.text = candidateData.candidate.yrsOfExperience.ToString();
        Cand_Education.text = candidateData.candidate.educationLvl.ToString();
        Cand_Longitude.text = Math.Round(candidateData.candidate.longitude, 5).ToString();
        Cand_Latitude.text = Math.Round(candidateData.candidate.latiture, 5).ToString();
        foreach (var pref in candidateData.candidate.sectorPreferences)
        {
            switch ((Sector)pref.sector)
            {
                case Sector.Administrative: Cand_Admin.value = pref.prefrence; break;
                case Sector.CustomerService: Cand_CustomerService.value = pref.prefrence; break;
                case Sector.Flex: Cand_Flex.value = pref.prefrence; break;
                case Sector.Food: Cand_Food.value = pref.prefrence; break;
                case Sector.Industry: Cand_Industry.value = pref.prefrence; break;
                case Sector.Trade: Cand_Trade.value = pref.prefrence; break;
            }
        }
        foreach (var pref in candidateData.realSectorPrefrence)
        {
            switch ((Sector)pref.sector)
            {
                case Sector.Administrative: ColorHidden(pref.prefrence, Cand_Admin_Hidden); break;
                case Sector.CustomerService: ColorHidden(pref.prefrence, Cand_CustomerService_Hidden); break;
                case Sector.Flex: ColorHidden(pref.prefrence, Cand_Flex_Hidden); break;
                case Sector.Food: ColorHidden(pref.prefrence, Cand_Food_Hidden); break;
                case Sector.Industry: ColorHidden(pref.prefrence, Cand_Industry_Hidden); break;
                case Sector.Trade: ColorHidden(pref.prefrence, Cand_Trade_Hidden); break;
            }
        }
    }

    private void ColorHidden(float pref, Label label)
    {
        const int midlevel = 50;
        label.text = pref.ToString();

        if (pref == midlevel) label.style.color = new Color(0, 0, 0, 0);
        if (pref > midlevel) label.style.color = positiveHidden;
        if (pref < midlevel) label.style.color = negativeHidden;
    }
}
