using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
  [Route("persons")]
  public class PersonsController : Controller
  {
    //private fields
    private readonly IPersonsService _personsService;
    private readonly ICountriesService _countriesService;

    //constructor
    public PersonsController(IPersonsService personsService, ICountriesService countriesService)
    {
        _personsService = personsService;
        _countriesService = countriesService;
    }

    // GET: persons
    [HttpGet("")]
    [HttpGet("index")]
    public IActionResult Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
    {
        //Search
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryID), "Country" },
            { nameof(PersonResponse.Address), "Address" }
        };
        List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy, searchString);
        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        //Sort
        List<PersonResponse> sortedPersons =  _personsService.GetSortedPersons(persons, sortBy, sortOrder);
        ViewBag.CurrentSortBy = sortBy;
        ViewBag.CurrentSortOrder = sortOrder.ToString();

        return View(sortedPersons); //Views/Persons/Index.cshtml
    }

    // GET: persons/create
    [HttpGet("create")]
    public IActionResult Create()
    {
        List<CountryResponse> countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp => 
            new SelectListItem() {  Text = temp.CountryName, Value = temp.CountryID.ToString() }
        );

        return View();
    }

    // POST: persons/create
    [HttpPost("create")]
    public IActionResult Create(PersonAddRequest personAddRequest)
    {
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View();
        }

        //call the service method
        PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
        
        //navigate to Index() action method (it makes another get request to "persons/index"
        return RedirectToAction("Index");
    }

    // GET: persons/edit/{personID}
    [HttpGet("edit/{personID}")]
    public IActionResult Edit(Guid personID)
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        List<CountryResponse> countries = _countriesService.GetAllCountries();
        ViewBag.Countries = countries.Select(temp =>
        new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        return View(personUpdateRequest);
    }

    // POST: persons/edit/{personID}
    [HttpPost("edit/{personID}")]
    public IActionResult Edit(PersonUpdateRequest personUpdateRequest)
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

        if (personResponse == null)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid)
        {
            PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);
            return RedirectToAction("Index");
        }
        else
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(personResponse.ToPersonUpdateRequest());
        }
    }

    // GET: persons/delete/{personID}
    [HttpGet("delete/{personID}")]
    public IActionResult Delete(Guid? personID)
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);
        if (personResponse == null)
            return RedirectToAction("Index");

        return View(personResponse);
    }

    // POST: persons/delete/{personID}
    [HttpPost("delete/{personID}")]
    public IActionResult Delete(PersonUpdateRequest personUpdateResult)
    {
        PersonResponse? personResponse = _personsService.GetPersonByPersonID(personUpdateResult.PersonID);
        if (personResponse == null)
            return RedirectToAction("Index");

        _personsService.DeletePerson(personUpdateResult.PersonID);
        return RedirectToAction("Index");
    }
  }
}
