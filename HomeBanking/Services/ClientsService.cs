using HomeBanking.DTOs;
using HomeBanking.Models;
using HomeBanking.Models.Enums;
using HomeBanking.Repositories;
using HomeBanking.Utils;
using Sqids;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace HomeBanking.Services
{
    public class ClientsService : IClientsService
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;
        private SqidsEncoder<long> _sqids;

        public ClientsService(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository, SqidsEncoder<long> sqids)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _sqids = sqids;
        }

        public IEnumerable<ClientDTO> GetAllClients()
        {
            IEnumerable<Client> clients = _clientRepository.GetAllClients();

            return clients.Select(client => new ClientDTO(client, _sqids));
        }

        public ClientDTO GetClientById(long id)
        {
            Client client = _clientRepository.FindById(id);

            if (client is null)
                throw new Exception("Cliente no encontrado");

            return new ClientDTO(client, _sqids);
        }

        public ClientDTO GetClientByEmail(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client is null)
                throw new Exception("Cliente no encontrado");

            return new ClientDTO(client, _sqids);
        }

        public Client CreateClient(ClientInDTO userDTO)
        {
            if (_clientRepository.ExistsByEmail(userDTO.Email))
                throw new Exception("Email está en uso");

            Client newClient = new Client
            {
                Email = userDTO.Email,
                Password = PasswordsUtils.HashPassword(userDTO.Password),
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Role = Regex.IsMatch(userDTO.Email, @".*@vinotinto\.com") ? RoleType.ADMIN : RoleType.CLIENT,
                Accounts = new List<Account>
                {
                    new Account()
                    {
                        Number = NewAccountNumber(),
                        CreationDate = DateTime.Now,
                        Balance = 0
                    }
                }
            };

            _clientRepository.Save(newClient);

            return newClient;
        }

        public AccountDTO CreateAccount(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client is null)
                throw new Exception("Cliente no encontrado");

            if (client.Accounts.Count() >= 3)
                throw new Exception("Cuentas maximas alcanzadas");

            Account account = new Account()
            {
                Number = NewAccountNumber(),
                CreationDate = DateTime.Now,
                Balance = 0
            };

            account.ClientId = client.Id;

            _accountRepository.Save(account);

            AccountDTO accountDTO = new AccountDTO(account, _sqids);

            return accountDTO;
        }

        public CardOutDTO CreateCard(string email, CardInDTO cardInDTO)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client is null)
                throw new Exception("Cliente no encontrado");

            CardType cardType = (CardType)Enum.Parse(typeof(CardType), cardInDTO.Type);
            CardColorType cardColor = (CardColorType)Enum.Parse(typeof(CardColorType), cardInDTO.Color);

            if (client.Cards.Any(card => card.Type == cardType && card.Color == cardColor))
                throw new Exception("Tarjeta denegada");

            string cardNumber = NewCardNumber();            

            Card card = new Card
            {
                ClientId = client.Id,
                CardHolder = client.FirstName + " " + client.LastName,
                Type = cardType,
                Color = cardColor,
                Number = cardNumber,
                Cvv = CardUtils.RandomNumber(4),
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
            };

            _cardRepository.Save(card);

            CardOutDTO cardOutDTO = new CardOutDTO(card);

            return cardOutDTO;
        }

        public IEnumerable<AccountDTO> GetAccounts(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client is null)
                throw new Exception("Cliente no encontrado");

            IEnumerable<AccountDTO> accountsDTO = client.Accounts.Select(account => new AccountDTO(account, _sqids));

            return accountsDTO;
        }
        public IEnumerable<CardOutDTO> GetCards(string email)
        {
            Client client = _clientRepository.FindByEmail(email);

            if (client == null)
                throw new Exception("Cliente no encontrado");

            IEnumerable<CardOutDTO> cardsDTO = client.Cards.Select(card => new CardOutDTO(card));

            return cardsDTO;
        }

        private string NewCardNumber()
        {
            string cardNumber;
            do
            {
                string firstCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0'); ;
                string secondCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0'); ;
                string thirdCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0'); ;
                string fourthCardNumbers = CardUtils.RandomNumber(4).ToString().PadLeft(4, '0'); ;

                List<string> lCardNumber = new List<string>();

                lCardNumber.Clear();

                lCardNumber.Add(firstCardNumbers);
                lCardNumber.Add(secondCardNumbers);
                lCardNumber.Add(thirdCardNumbers);
                lCardNumber.Add(fourthCardNumbers);

                cardNumber = string.Join("-", lCardNumber);
            }
            while (_cardRepository.ExistsCardNumber(cardNumber));

            return cardNumber;
        }

        private string NewAccountNumber()
        {
            string accountNumber;

            do
            {
                accountNumber = ("VIN-" + CardUtils.RandomNumber(8)).PadLeft(8, '0'); ;
            }
            while (_accountRepository.ExistsAccountNumber(accountNumber));

            return accountNumber;
        }
    }
}