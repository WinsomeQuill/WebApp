namespace WebApp.Models;

public class PersonDto
{
    public string Name { get; set; }
    
    public string DisplayName { get; set; }
    
    public List<SkillDto> Skills { get; set; }
}