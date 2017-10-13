using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Server : MonoBehaviour
{

    Chat chatScript;
    Chat chatPlayer;

    Places startPlace;

    public List<Chat> playerList = new List<Chat>(4);
    public List<Chat> playerListRemovidos = new List<Chat>();
    public List<Places> placeonWorld = new List<Places>();
    public List<Itens> itensonWorld = new List<Itens>();

    public string placeN;
    public int auxItem;

    // Use this for initialization
    void Start()
    {
        chatScript = GetComponent<Chat>();
        CreateWorld();
    }

    void CreateWorld()
    {
        //Places

        
        string aux1 = "Você está no sótão, este lugar cheira a mofo! Você só enxerga tralhas por aqui. Talvez encontre algo útil.. Sua única saida é pela escada (baixo).";
        placeonWorld.Add(new Places(1, aux1, "Sótão", 1, 0, 0, 0, 0, 2, 0, 0));
        Debug.Log("Sótão CREATED");

        string aux2 = "Você está em um quarto todo empoeirado, com rastros de passagem até a porta. Aqui você pode voltar para o sótão (baixo) ou passar pela porta (cima).";
        placeonWorld.Add(new Places(2, aux2, "Quarto", 0, 0, 0, 0, 3, 1, 0, 0));
        Debug.Log("Quarto CREATED");

        string aux3 = "Neste corredor bizarro você só consegue enxergar as conexões que ele faz: porta de madeira simples (baixo), porta de madeira entalhada (cima) e na direção (direita) você acaba indo a algum lugar...";
        placeonWorld.Add(new Places(3, aux3, "Corredor", 0, 4, 2, 0, 4, 2, 5, 0));
        Debug.Log("Corredor CREATED");

        string aux4 = "Neste quarto você sente uma presença aterrorizante. O quarto possui uma suíte e uma mobília rústica que pode ser examinada. Sua mente diz que você precisa sair logo desse local! A única saída é por onde entrou (baixo).";
        placeonWorld.Add(new Places(4, aux4, "Suíte", 3, 0, 0, 2, 0, 3, 0, 0));
        Debug.Log("Suíte CREATED");

        string aux5 = "Você está na Sala de Jantar. O que te deixa mais confuso, pois o local está todo iluminado e um banquete está servido. Deste local você enxerga o que pode ser um escritório (baixo), o corredor dos quartos (esquerda) e para (cima) é algum outro cômodo da casa.";
        placeonWorld.Add(new Places(5, aux5, "Sala de Jantar", 0, 0, 0, 0, 6, 7, 0, 3));
        Debug.Log("Sala de Jantar CREATED");

        string aux6 = "Você acaba de entrar na cozinha, e ela está toda suja e bagunçada. Parece que não é tocada por décadas... o cheiro de podridão está em todo lugar. Deste local você consegue acessar uma sala (direita) e a sala de jantar (esquerda).";
        placeonWorld.Add(new Places(6, aux6, "Cozinha", 0, 0, 0, 0, 0, 0, 8, 5));
        Debug.Log("Cozinha CREATED");

        string aux7 = "Neste escritório todo empoeirado, você só consegue enxergar livros e papéis em cima da única mesa. Sua única saída é por onde veio (cima).";
        placeonWorld.Add(new Places(7, aux7, "Escritório", 0, 0, 0, 0, 5, 0, 0, 0));
        Debug.Log("Escritório CREATED");

        string aux8 = "A sala de estar é bem grande e possui uma grande variedade de mobílias, com gavetas e prateleiras que podem ser examinadas. Você pode voltar para a cozinha (esquerda) ou ir para uma porta que está toda torta (baixo). Esta porta precisa ser reparada com algo.";
        placeonWorld.Add(new Places(8, aux8, "Sala de Estar", 2, 9, 1, 0, 0, 9, 0, 6));
        Debug.Log("Sala de Estar CREATED");

        string aux9 = "Você está no bar da casa... aqui você só encontra garrafas que estão vazias e copos quebrados. Você consegue acessar a sala de estar (cima) e o que parece ser um Hall enorme (esquerda).";
        placeonWorld.Add(new Places(9, aux9, "Bar", 0, 0, 0, 1, 8, 0, 0, 10));
        Debug.Log("Bar CREATED");

        string aux10 = "Você está no Hall de Entrada, mas este local está todo escuro! Você consegue enxergar a porta que parece ser a saída (baixo) e pela (direita) você tem acesso ao bar.";
        placeonWorld.Add(new Places(10, aux10, "Hall de Entrada", 0, 11, 3, 0, 0, 11, 9, 0));
        Debug.Log("Hall de Entrada CREATED");

        string aux11 = "Fora da Casa";
        placeonWorld.Add(new Places(11, aux11, "Fora da Casa", 0, 0, 0, 3, 0, 0, 0, 0));
        Debug.Log("Fora da Casa CREATED");

        //Itens
        itensonWorld.Add(new Itens(1, "Alavanca"));
        Debug.Log("Alavanca ITEM CREATED");
        itensonWorld.Add(new Itens(2, "Chave Prateada"));
        Debug.Log("Chave Prateada CREATED");
        itensonWorld.Add(new Itens(3, "Chave Dourada"));
        Debug.Log("Chave Dourada CREATED");
        //itensonWorld.Add(new Itens(4, "Mapa"));
        //Debug.Log("Mapa");
    }

    void OnConnectedToServer()
    {
        chatScript.ShowChatWindow();
        GetComponent<NetworkView>().RPC("TellServerOurName", RPCMode.Server);
    }

    void OnServerInitialized()
    {
        chatScript.ShowChatWindow();
    }

    void OnPlayerDisconnected(NetworkPlayer netPlayer)
    {
          Chat playerDisc = WhoSend(netPlayer);     
        
          //drop player item on disconnect
          if (playerDisc.myItens.idItem != 0)
         {
             for (int i = 0; i < placeonWorld.Count; i++)
             {
                 if (placeonWorld[i].idPlace == playerDisc.playerAtThisPlace.idPlace)
                 {
                     string aux = "O jogador(a): (" + playerDisc.playerName;
                     GetComponent<NetworkView>().RPC("AddChatMessage", RPCMode.Server, "Server", aux + ") se desconectou e deixou o item: " + playerDisc.myItens.itemName + " antes de sair.");
                     placeonWorld[i].hasItem = playerDisc.myItens.idItem;
                     Disconnected(playerDisc);                   
                }
             }
         }
         else
         {
             GetComponent<NetworkView>().RPC("AddChatMessage", RPCMode.Server, "Server", "O jogador (" + playerDisc.playerName + ") se desconectou.");
             Disconnected(playerDisc);
        } 
    }
        
    void Disconnected(Chat player)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if(playerList[i].playerName == player.playerName)
            {
                playerList[i].playerName = "NULL";
                playerList[i].playerAtThisPlace.idPlace = 0;
            }
        }
    }



    Places GetStartPlace()
    {
        //TESTE DO PLACE
        for (int i = 0; i < placeonWorld.Count; i++)
        {
            if (placeonWorld[i].idPlace == 1)
            {
                return placeonWorld[i];
                //Debug.Log("RETORNOU O LOCAL DO PLAYER");
            }
        }
        return null;
    }

    [RPC]
    //Sent by newly connected clients, recieved by server
    public void TellServerOurName(NetworkMessageInfo info)
    {
        // Debug.Log("TellServerOurName");
        GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Bem-vindo. Digite seu nome!");
    }

    [RPC]
    void NewPlayerList(string nome, NetworkMessageInfo info)
    {
        bool exist = false;

        if(playerList.Count > 0)
        {
            foreach (Chat c in playerList)
            {
                if (c.playerName.CompareTo(nome) == 0 || nome.ToLower().CompareTo("server") == 0)
                {
                    Debug.Log("Achou um com mesmo nome");
                    exist = true;
                    //break;
                }
            }
        }
        Debug.Log("exist: " +  exist);
        Debug.Log("exist == false: " + (exist == false));
        if (exist == false)
        {
            Chat p = new Chat();
            p.playerName = nome;
            p.networkPlayer = info.sender;

            playerList.Add(p);
            p.playerAtThisPlace = GetStartPlace();
            p.myItens = new Itens(0, "NULL");
            GetComponent<NetworkView>().RPC("AddChatMessage", p.networkPlayer, "Server", "Nome alterado com sucesso! ");
            GetComponent<NetworkView>().RPC("AddChatMessage", RPCMode.All, "Server", "O Jogador(a): " + p.playerName + " se conectou!");
            Welcome(info.sender);
        }
        else
        {
            
            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "O nome selecionado ja está em uso por outro jogador! Escolha outro.");
        }
    }

    [RPC]
    public void Interpret(string cmd, string name, NetworkMessageInfo info)
    {
        string comands = cmd[0].ToString();
        Debug.Log("Interpret Sender: " + info.sender);
        //Chat chatPlay = WhoSend(info.sender);
        Chat chatPlay = WhoSendName(name);
        Debug.Log("Interpret ChatPlay: " + chatPlay.playerName);
        

        int oldIdPlace = chatPlay.playerAtThisPlace.idPlace;
        Debug.Log(oldIdPlace);
        //Commands
        if (comands == "/")
            {
                switch (cmd.ToLower())
                {
                    case "/cima":
                        {
                            int idMoveAux = chatPlay.playerAtThisPlace.north;

                            if (chatPlay.playerAtThisPlace.north == 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Não é possível ir na direção desejada.");
                                return;
                            }
                            else
                            {
                                for (int j = 0; j < placeonWorld.Count; j++)
                                {
                                    if (placeonWorld[j].idPlace == idMoveAux)
                                    {
                                        if (placeonWorld[j].needItemToPass > 0)
                                        {
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Para passar por esta porta você precisa destrancá-la de alguma maneira.");
                                        }
                                        else
                                        {
                                            //Atualiza o local do personagem.
                                            chatPlay.playerAtThisPlace = placeonWorld[j];
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", placeonWorld[j].placeDescription);

                                            //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                            for (int i = 0; i < playerList.Count; i++)
                                            {
                                                if (playerList[i].playerAtThisPlace.idPlace == oldIdPlace)
                                                {
                                                    //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                                    GetComponent<NetworkView>().RPC("AddChatMessage", playerList[i].networkPlayer, "Server", "O jogador(a) " + chatPlay.playerName + " saiu do seu local indo para a direção (cima).");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "/baixo":
                        {
                            int idMoveAux = chatPlay.playerAtThisPlace.south;
                            
                            if (chatPlay.playerAtThisPlace.south == 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Não é possível ir na direção desejada.");
                                return;
                            }
                            else
                            {
                                for (int j = 0; j < placeonWorld.Count; j++)
                                {
                                    if (placeonWorld[j].idPlace == idMoveAux)
                                    {
                                        if (placeonWorld[j].needItemToPass > 0)
                                        {
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Para passar por esta porta você precisa destrancá-la de alguma maneira.");
                                        }
                                        else
                                        {
                                            //Atualiza o local do personagem.
                                            chatPlay.playerAtThisPlace = placeonWorld[j];
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", placeonWorld[j].placeDescription);

                                            if(chatPlay.playerAtThisPlace.idPlace == 11)
                                            {
                                                 EndGame(info.sender);
                                            }

                                            //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                            for (int i = 0; i < playerList.Count; i++)
                                            {
                                                if (playerList[i].playerAtThisPlace.idPlace == oldIdPlace)
                                                {
                                                    //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                                    GetComponent<NetworkView>().RPC("AddChatMessage", playerList[i].networkPlayer, "Server", "O jogador(a) " + chatPlay.playerName + " saiu do seu local indo para a direção (baixo).");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "/direita":
                        {
                            int idMoveAux = chatPlay.playerAtThisPlace.east;

                            if (chatPlay.playerAtThisPlace.east == 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Não é possível ir na direção desejada.");
                                return;
                            }
                            else
                            {
                                for (int j = 0; j < placeonWorld.Count; j++)
                                {
                                    if (placeonWorld[j].idPlace == idMoveAux)
                                    {
                                        if (placeonWorld[j].needItemToPass > 0)
                                        {
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Para passar por esta porta você precisa destrancá-la de alguma maneira.");
                                        }
                                        else
                                        {
                                            //Atualiza o local do personagem.
                                            chatPlay.playerAtThisPlace = placeonWorld[j];
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", placeonWorld[j].placeDescription);

                                            //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                            for (int i = 0; i < playerList.Count; i++)
                                            {
                                                if (playerList[i].playerAtThisPlace.idPlace == oldIdPlace)
                                                {
                                                    //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                                    GetComponent<NetworkView>().RPC("AddChatMessage", playerList[i].networkPlayer, "Server", "O jogador(a) " + chatPlay.playerName + " saiu do seu local indo para a direção (direita).");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "/esquerda":
                        {
                            int idMoveAux = chatPlay.playerAtThisPlace.west;

                            if (chatPlay.playerAtThisPlace.west == 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Não é possível ir na direção desejada.");
                                return;
                            }
                            else
                            {
                                for (int j = 0; j < placeonWorld.Count; j++)
                                {
                                    if (placeonWorld[j].idPlace == idMoveAux)
                                    {
                                        if (placeonWorld[j].needItemToPass > 0)
                                        {
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Para passar por esta porta você precisa destrancá-la de alguma maneira.");
                                        }
                                        else
                                        {
                                            //Atualiza o local do personagem.
                                            chatPlay.playerAtThisPlace = placeonWorld[j];
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", placeonWorld[j].placeDescription);

                                            //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                            for (int i = 0; i < playerList.Count; i++)
                                            {
                                                if (playerList[i].playerAtThisPlace.idPlace == oldIdPlace)
                                                {
                                                    //MENSAGEM PARA QUEM TA NO MESMO LOCAL
                                                    GetComponent<NetworkView>().RPC("AddChatMessage", playerList[i].networkPlayer, "Server", "O jogador(a) " + chatPlay.playerName + " saiu do seu local indo para a direção (esquerda).");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "/onde":
                        {
                            for (int i = 0; i < playerList.Count; i++)
                            {
                                if (playerList[i].networkPlayer == info.sender)
                                {
                                    GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", playerList[i].playerAtThisPlace.placeDescription);
                                    return;
                                }
                            }
                        }
                        break;

                    case "/examinar":
                        {
                            if (chatPlay.playerAtThisPlace.idPlace == 7)
                            {
                                chatPlay.hasMap = true;
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Mexendo nos papéis em cima da mesa, você acaba encontrando a planta da casa. Agora você pode utilizar o comando [/vermapa ] para se localizar.");
                            }
                            else
                            {
                                for (int i = 0; i < placeonWorld.Count; i++)
                                {
                                    if (placeonWorld[i].idPlace == chatPlay.playerAtThisPlace.idPlace)
                                    {
                                        if (placeonWorld[i].hasItem == 0)
                                        {
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Aqui não possui nenhum item que possa ser coletável.");
                                        }
                                        else
                                        {
                                            for (int j = 0; j < itensonWorld.Count; j++)
                                            {
                                                int itemaux = placeonWorld[i].hasItem;

                                                if (itensonWorld[j].idItem == itemaux)
                                                {
                                                    GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Itens que podem ser coletáveis: " + itensonWorld[j].itemName);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;

                    case "/pegar":
                        {
                            if (chatPlay.myItens.idItem > 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Seu inventário está cheio ou aqui não possui itens coletáveis.");
                                return;
                            }

                            for (int i = 0; i < placeonWorld.Count; i++)
                            {
                                if (placeonWorld[i].idPlace == chatPlay.playerAtThisPlace.idPlace)
                                {
                                    if (placeonWorld[i].hasItem == 0)
                                    {
                                        GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Este local não possui itens coletáveis");
                                    }
                                    else
                                    {
                                        int itemaux = placeonWorld[i].hasItem;

                                        for (int j = 0; j < itensonWorld.Count; j++)
                                        {
                                            if (itensonWorld[j].idItem == itemaux)
                                            {
                                                chatPlay.myItens = itensonWorld[j];
                                                placeonWorld[i].hasItem = 0;
                                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Você pegou o item: " + itensonWorld[j].itemName);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        break;

                    case "/largar":
                        {
                            for (int i = 0; i < placeonWorld.Count; i++)
                            {
                                if (placeonWorld[i].idPlace == chatPlay.playerAtThisPlace.idPlace)
                                {
                                    if (placeonWorld[i].hasItem >= 1)
                                    {
                                        GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Este item não pode ser colocado neste local. Procure outra sala para largar o item.");
                                        return;
                                    }
                                    else
                                    {
                                        //Message for all players in ROOM
                                        for (int j = 0; j < playerList.Count; j++)
                                        {
                                            if (playerList[j].playerAtThisPlace.idPlace == chatPlay.playerAtThisPlace.idPlace)
                                            {
                                                //MENSAGEM PARA QUEM TA NO ID 1
                                                GetComponent<NetworkView>().RPC("AddChatMessage", playerList[j].networkPlayer, "Server", "O Jogador(a) (" + chatPlay.playerName + ") deixou o item " + chatPlay.myItens.itemName + " no chão.");
                                            }
                                        }

                                        placeonWorld[i].hasItem = chatPlay.myItens.idItem;
                                        chatPlay.myItens = new Itens(0, "NULL");
                                        return;
                                    }
                                }
                            }
                        }
                        break;

                    //   placeonWorld[i].hasItem = chatPlay.myItens.idItem;
                    //   chatPlay.myItens = new Itens(0, "NULL");
                    //   return;
                    //
                    case "/usar":
                        {
                            if (chatPlay.myItens.idItem == 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Você não possui itens para utilizar!");
                                return;
                            }
                            else
                            {
                                for (int i = 0; i < placeonWorld.Count; i++)
                                {
                                    if (placeonWorld[i].idPlace == chatPlay.playerAtThisPlace.idPlace)
                                    {
                                        if (placeonWorld[i].doorCloseItem == chatPlay.myItens.idItem)
                                        {
                                            auxItem = placeonWorld[i].hasDoorClose;

                                            placeonWorld[i].hasDoorClose = 0;
                                            placeonWorld[i].doorCloseItem = 0;
                                            OpenDoor(chatPlay.myItens.idItem);
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Você destrancou a porta.");
                                            return;
                                        }
                                        else
                                        {
                                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Seu item não tem serventia nesta sala!");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                        break;


                    case "/item":
                        {
                            if (chatPlay.myItens.idItem == 0)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Você não possui nenhum item.");
                            }
                            else
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Você possui o item " + chatPlay.myItens.itemName);
                            }
                        }
                        break;

                    case "/ajuda":
                        {
                            string aux = "Comandos: \n'";
                            aux = aux + "Examinar [/examinar] - Procurar itens que possam ser coletados. \n'";
                            aux = aux + "Mover [/cima /baixo /direita /esquerda] - Move-se nas direções. \n'";
                            aux = aux + "Descrição do local [/onde] - Descreve o local onde você se encontra. \n'";
                            aux = aux + "Pegar [/pegar] - Pegar item coletável. \n'";
                            aux = aux + "Inventário [/item] - Listar o item que você possui. \n'";
                            aux = aux + "Largar [/largar] - Largar item no local onde está.\n'";
                            aux = aux + "Usar [/usar] - Utiliza o item que você pegou (normalmente para abrir portas específicas). \n'";
                            aux = aux + "Cochichar [/cochichar - TEXTO - JOGADOR] - Mandar mensagem privada para alguém da mesma sala. (utilizar - para montar o comando)\n'";
                            aux = aux + "Jogadores [/jogadorsala] - Jogadores que estão na mesma sala que você.\n'";
                            aux = aux + "Mapa [/vermapa] - Ver mapa o do local.\n";
                            GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", aux);
                            return;
                        }

                    case "/vermapa":
                        {
                            if (chatPlay.hasMap == false)
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Server", "Você não tem conhecimento do local!");
                            }
                            else
                            {
                                MapUse(info.sender);
                                return;
                            }
                            return;
                        }

                     case "/jogadorsala":
                        {
                            PlayersOnline(chatPlay);
                            return;
                        }


                    default:
                        {
                            ///cochichar - frase - jogador
                            //////cochichar-frase-jogador
                            string[] aux = cmd.Split('-');
                            //Debug.Log("aux[0]: [" + aux[0].Trim() + "]");
                            //Debug.Log("aux[1]: [" + aux[1].Trim() + "]");
                            //Debug.Log("aux[2]: [" + aux[2].Trim() + "]");
                            //Debug.Log("aux[0].ToLower(): [" + aux[0].ToLower() + "]");

                            if (aux[0].ToLower().Trim() == "/cochichar")
                            {
                                Chat c = WhoSendName(aux[2].ToString().ToLower().Trim());

                                if (c == null)
                                {
                                    Chat myChat = WhoSend(info.sender);
                                    if (myChat.playerAtThisPlace == c.playerAtThisPlace)
                                    {
                                        GetComponent<NetworkView>().RPC("AddChatMessage", c.networkPlayer, myChat.playerName + " Cochichou para você", aux[1].Trim());
                                        GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "Cochichou para " + c.playerName, aux[1].Trim());
                                        return;
                                    }
                                    else
                                    {
                                        GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "SERVER", "COMANDO INVALIDO!, JOGADOR NÃO ESTÁ NA MESMA SALA");
                                        return;
                                    }
                                }
                                else
                                {
                                    GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "SERVER", "COMANDO INVALIDO!, JOGADOR NÃO LOCALIZADO");
                                    return;
                                }
                            }
                            else
                            {
                                GetComponent<NetworkView>().RPC("AddChatMessage", info.sender, "SERVER", "COMANDO INVALIDO!");
                                return;
                            }
                        }
                }
        }
        else
        {
            //Message for all players in ROOM
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].playerAtThisPlace.idPlace == chatPlay.playerAtThisPlace.idPlace)
                {
                    
                        Debug.Log("teste: " + playerList[i].networkPlayer);
                        //MENSAGEM PARA QUEM TA NO ID 1
                        GetComponent<NetworkView>().RPC("AddChatMessage", playerList[i].networkPlayer, name, cmd);
                    
                }
            }
        }
        
                       
    }     

    void OpenDoor(int idItem)
    {
        for(int i = 0; i < placeonWorld.Count; i++)
        {
            if(placeonWorld[i].needItemToPass == idItem)
            {
                placeonWorld[i].needItemToPass = 0;
            }
        }
    }

    Chat WhoSend(NetworkPlayer netplayer)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].networkPlayer == netplayer)
            {
                Debug.Log("Retornou chat - Netplayer: " + netplayer.ToString() + " playerList["+i+"]: " + playerList[i]);
                return playerList[i];
            }
        }
        Debug.Log("Retornou null - Netplayer: " + netplayer.ToString());
        return null;
    }

	Chat WhoSendName(string nome)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerName == nome)
            {
                Debug.Log("Retornou o player: " + playerList[i].playerName);
                return playerList[i];
            }
        }
        Debug.Log("Retornou player null");
        return null;
    }
	
	public NetworkPlayer Gets(Chat player)
    {
        foreach(Chat p in playerList)
        {
            if(p.playerName == player.name)
            {
                return p.networkPlayer;
            }
        }
        return player.networkPlayer;        
    }

    public void MapUse(NetworkPlayer netPlayer)
    {
        string auxMap = "\n#######################################################\n";
        auxMap = auxMap + "#                           ##                   6                      #                   #\n";
        auxMap = auxMap + "#            4             ##################               #                   #\n";
        auxMap = auxMap + "#                           ##                           ##########         8        #\n";
        auxMap = auxMap + "##################       5                   ##########                   #\n";
        auxMap = auxMap + "#            3              ##              #################                   #\n";
        auxMap = auxMap + "############################                           #######       #\n";
        auxMap = auxMap + "#                           ##             ##                          #          #       #\n";
        auxMap = auxMap + "#            2             ##             ##                          #    9    #        #\n";
        auxMap = auxMap + "#                           ##     7      ##         10              #         #        #\n";
        auxMap = auxMap + "#          ###           ##             ##                           #         #        #\n";
        auxMap = auxMap + "####### 1 ##############################################\n\n";
        auxMap = auxMap + "1 - Sótão, 2 - Quarto, 3 - Corredor, 4 - Suíte, 5 - Sala de Jantar, 6 - Cozinha, 7 - Escritório, 8 - Sala de Estar, 9 - Bar, 10 - Hall de Entrada";

        GetComponent<NetworkView>().RPC("AddChatMessage", netPlayer, "Server", auxMap);
        
    }

    public void SetPlace(int newIdPlace, Chat player)
    {
        NetworkPlayer aux = Gets(player);

        GetComponent<NetworkView>().RPC("AddChatMessage", aux, player.playerName, "ENTROU NO SET");
       if (player.playerAtThisPlace.idPlace == newIdPlace)
       {
           GetComponent<NetworkView>().RPC("AddChatMessage", aux, player.playerName, "VOCE JA ESTÁ AQUI");
           return;
       }
       else
       {
           for (int j = 0; j < placeonWorld.Count; j++)
           {
               if (placeonWorld[j].idPlace == newIdPlace)
               {
                   player.playerAtThisPlace = placeonWorld[j];
                   GetComponent<NetworkView>().RPC("AddChatMessage", aux, player.playerName, "ENTROU NO SET E MANDOU O MALUCO PRA FRENTE");
               }
           }
       }
    }

    void Welcome(NetworkPlayer NetPlayer)
    {
        string auxwelcome = "Você acaba de acordar em um lugar escuro sem saber o que está fazendo ali. A Única certeza que você tem é que você precisa sair desse lugar!\n\n\nDigite /ajuda para ver os comandos do jogo.";
        GetComponent<NetworkView>().RPC("AddChatMessage", NetPlayer, "Server", "Bem-vindo ao jogo Get Out! \n" + auxwelcome);        
    }

    void EndGame(NetworkPlayer NetPlayer)
    {
        Chat endPlayer = WhoSend(NetPlayer);

        string auxendgame = "Sem muitas explicações você consegue sair desse local estranho!";
        GetComponent<NetworkView>().RPC("AddChatMessage", NetPlayer, "Server", auxendgame);
        GetComponent<NetworkView>().RPC("AddChatMessage", RPCMode.Server, "Server", "O Jogador(a) " + endPlayer.playerName + " consegue escapar da mansão!");
    }

    void PlayersOnline(Chat chatplayer)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].playerAtThisPlace.idPlace == chatplayer.playerAtThisPlace.idPlace)
            {              
                GetComponent<NetworkView>().RPC("AddChatMessage", chatplayer.networkPlayer, "Server", "Jogadores na sala: " + playerList[i].playerName);                               
            }
           // else
           // {
           //     //GetComponent<NetworkView>().RPC("AddChatMessage", chatplayer.networkPlayer, "Server", "Não existe nenhum jogador nesta sala!");
           //     return;
           // }
        }       
    }
}


