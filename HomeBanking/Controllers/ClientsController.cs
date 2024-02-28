using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Models.Enums;
using HomeBanking.Repositories;
using HomeBanking.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;


        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number
                        }).ToList(),
                        Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString()
                        }).ToList()
                    };
                    clientsDTO.Add(newClientDTO);
                }
                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO

                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
        {
            try
            {
                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost()]
        public IActionResult Post([FromBody] UserDTO client)
        {
            try
            {
                //validamos datos antes
                if (string.IsNullOrEmpty(client.Email))
                    return StatusCode(403, "email inválido");

                if (string.IsNullOrEmpty(client.Password))
                    return StatusCode(403, "contrseña inválida");

                if (string.IsNullOrEmpty(client.FirstName))
                    return StatusCode(403, "nombre inválido");

                if (string.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "apellido inválido");

                //buscamos si ya existe el usuario
                if (_clientRepository.ExistsByEmail(client.Email))
                {
                    return StatusCode(403, "Email está en uso");
                }

                int accountNumber = CardUtils.RandomNumber(8);

                while (_accountRepository.ExistsAccountNumber((string)"VIN-".Concat(accountNumber.ToString())))
                {
                    accountNumber = CardUtils.RandomNumber(8);
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = new Account[]
                    {
                        new Account
                        {
                            Number = (string)"VIN-".Concat(accountNumber.ToString()),
                            CreationDate = DateTime.Now,
                            Balance = 0
                        }
                    }
                };

                _clientRepository.Save(newClient);
                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post()
        {
            try
            {
                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                if (client.Accounts.Count() >= 3)
                {
                    return StatusCode(403, "Cuenta Denegada");
                }

                int accountNumber = CardUtils.RandomNumber(8);

                string actualAccountNumber = string.Concat("VIN-", accountNumber.ToString());

                while (_accountRepository.ExistsAccountNumber(actualAccountNumber))
                {
                    accountNumber = CardUtils.RandomNumber(8);

                    actualAccountNumber = string.Concat("VIN-", accountNumber.ToString());
                }

                Account account = new Account()
                {
                    Number = actualAccountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id
                };

                _accountRepository.Save(account);

                AccountDTO accountDTO = new AccountDTO()
                {
                    Id = account.Id,
                    Balance = account.Balance,
                    CreationDate = account.CreationDate,
                    Number = account.Number
                };

                return Created("", accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult Post([FromBody] CardTypeColorDTO cardDTO)
        {
            try
            {
                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                CardType cardType = (CardType)Enum.Parse(typeof(CardType), cardDTO.Type);
                CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), cardDTO.Color);

                if (client.Cards.Any(card => card.Type == cardType && card.Color == cardColor))
                {
                    return StatusCode(403, "Tarjeta Denegada");
                }

                var firstCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0');
                var secondCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0');
                var thirdCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0');
                var fourthCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0');

                var lCardNumber = new List<string>();

                lCardNumber.Add(firstCardNumbers);
                lCardNumber.Add(secondCardNumbers);
                lCardNumber.Add(thirdCardNumbers);
                lCardNumber.Add(fourthCardNumbers);

                var actualCardNumber = string.Join("-", lCardNumber);

                while (_cardRepository.ExistsCardNumber(actualCardNumber))
                {
                    firstCardNumbers = CardUtils.RandomNumber(8).ToString();
                    fourthCardNumbers = CardUtils.RandomNumber(8).ToString();
                    fourthCardNumbers = CardUtils.RandomNumber(8).ToString();
                    fourthCardNumbers = CardUtils.RandomNumber(8).ToString();

                    lCardNumber.Clear();

                    lCardNumber.Add(firstCardNumbers);
                    lCardNumber.Add(secondCardNumbers);
                    lCardNumber.Add(thirdCardNumbers);
                    lCardNumber.Add(fourthCardNumbers);

                    actualCardNumber = string.Join("-", lCardNumber);
                }

                Card card = new Card
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = cardType,
                    Color = cardColor,
                    Number = actualCardNumber,
                    Cvv = CardUtils.RandomNumber(4),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                };

                _cardRepository.Save(card);

                CardDTO outCardDTO = new CardDTO
                {
                    Id = card.Id,
                    CardHolder = card.CardHolder,
                    Color = card.Color.ToString(),
                    Cvv = card.Cvv,
                    FromDate = card.FromDate,
                    Number = card.Number,
                    ThruDate = card.ThruDate,
                    Type = card.Type.ToString()
                };

                return Created("", outCardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentClientAccounts()
        {
            try
            {
                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                List<AccountDTO> accountsDTO = 
                    client.Accounts.Select(account => 
                    new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreationDate,
                        Balance = account.Balance,
                    }).ToList();
                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrentClientCards()
        {
            try
            {
                if (User.FindFirst("Client") == null && User.FindFirst("Admin") == null)
                {
                    return Forbid();
                }

                string email = User.FindFirst("Client") == null ? User.FindFirst("Admin").Value : User.FindFirst("Client").Value;

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                List<CardDTO> cardsDTO =
                    client.Cards.Select(card =>
                    new CardDTO
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Color = card.Color.ToString(),
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        Number = card.Number,
                        ThruDate = card.ThruDate,
                        Type = card.Type.ToString()
                    }).ToList();
                return Ok(cardsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

