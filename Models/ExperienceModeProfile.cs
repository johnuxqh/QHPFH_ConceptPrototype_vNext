namespace QHPFH_ConceptPrototype.Models;

public sealed record ExperienceModeProfile(
    PrototypeExperienceMode Mode,
    string BadgeLabel,
    string Summary,
    ExperienceDensityMode DensityMode,
    ExperienceInteractionMode InteractionMode,
    ExperienceInformationMode InformationMode,
    string CardDensity,
    string OperationalDensity,
    string PreferredSlideoutDensity,
    string PreferredPanelMode);
