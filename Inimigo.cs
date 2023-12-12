namespace jogoInicial
{
    public enum EnumInimigos {
        tipo1 = 1,
        tipo2,
        tipo3,
        tipo4,
        tipo5
    }
    public class Inimigo
    {
        static public TipoInimigo tipoInimigo1 = new("XX");
        static public TipoInimigo tipoInimigo2 = new(")(");
        static public TipoInimigo tipoInimigo3 = new(";;");
        static public TipoInimigo tipoInimigo4 = new("");
        static public TipoInimigo tipoInimigo5 = new("@@");
        
        // Variacao da movimentação do inimigo
        static public List<List<int>> altPosicaoNormal = new List<List<int>>{
            new List<int>{-1, 0}, // cima
            new List<int>{1, 0}, // baixo
            new List<int>{0, 1}, // direita
            new List<int>{0, -1}, // esquerda
        };

        static public List<List<int>> altPosicaoComDiagonal = new List<List<int>>{
            new List<int>{-1, 0}, // cima
            new List<int>{1, 0}, // baixo
            new List<int>{0, 1}, // direita
            new List<int>{0, -1}, // esquerda
            new List<int>{-1, -1}, // cima - esquerda
            new List<int>{-1, 1}, // cima - direita
            new List<int>{1, -1}, // baixo - esquerda
            new List<int>{1, 1}, // baixo - direita
        };

        public static void MovimentacaoInimigo (TipoInimigo tipoInimigo){
            int[,] posicoesInimigos = new int[Game.GetMapa().GetLength(0) * Game.GetMapa().GetLength(1), 2];

            int qntInimigos = 0;

            for (int i = 0; i < Game.GetMapa().GetLength(0); i++){
                for (int j = 0; j < Game.GetMapa().GetLength(1); j++){
                    if(tipoInimigo._aparencia == Game.GetMapa()[i,j]){
                        posicoesInimigos[qntInimigos, 0] = i;
                        posicoesInimigos[qntInimigos, 1] = j;
                        qntInimigos++;
                    }
                }
            }

            static string limpaLugarAntigoInimigo(int[,] posicaoInimigo, int i) => 
                Game.GetMapa()[posicaoInimigo[i, 0], posicaoInimigo[i, 1]] = "  ";

            int limitaMovimento = tipoInimigo._aparencia == ";;" ? 0 : 4;

            for(int i = 0; i < qntInimigos; i++){
                var numeroAleatorio = new Random();
                List<List<int>> cloneAltPosicao = new List<List<int>>(
                    tipoInimigo._aparencia == ";;" 
                    ? altPosicaoComDiagonal
                    : altPosicaoNormal
                );
                
                for(int c = 0; c < cloneAltPosicao.Count; c++) {
                    int idxAleatorio = numeroAleatorio.Next(0, cloneAltPosicao.Count);
                    string destino = Game.GetMapa()[
                        cloneAltPosicao.ElementAt(idxAleatorio).ElementAt(0) + posicoesInimigos[i, 0],
                        cloneAltPosicao.ElementAt(idxAleatorio).ElementAt(1) + posicoesInimigos[i, 1]
                    ];

                    void acaoInimigo(bool destroiItem = false) => 
                        Game.GetMapa()[ 
                            cloneAltPosicao.ElementAt(idxAleatorio).ElementAt(0) + posicoesInimigos[i, 0],
                            cloneAltPosicao.ElementAt(idxAleatorio).ElementAt(1) + posicoesInimigos[i, 1]
                        ] = destroiItem 
                            ? 
                            "()" 
                            : 
                            tipoInimigo._aparencia;
                    
                    if(
                        destino == "  " || 
                        DB.todosTiposItens.FindIndex(i => i._modelo == destino) >= 0
                    ){
                        if (tipoInimigo._aparencia != "@@") {
                            limpaLugarAntigoInimigo(posicoesInimigos, i);
                        } else {
                            Game.FaseAtual._qntInimigosTipo5++;
                        }
                        acaoInimigo();
                        break;
                        
                    } else if(destino == "[}"){
                        // Procura inimigo nos tipos para matar, precisa de index NAO aparencia
                        int idtipoInimigo = DB.todosTiposInimigo
                            .FindIndex(inimigo => 
                                inimigo == tipoInimigo._aparencia)
                            + 1;
                        Personagem.MatarInimigo((EnumInimigos)idtipoInimigo);
                        Personagem.inventario.escudo._equipado = false;
                        Personagem.inventario.espada.nmrPuloAtaqueValido = 0;
                        acaoInimigo(true);
                        limpaLugarAntigoInimigo(posicoesInimigos, i);
                        break;

                    } else if(destino == "{}"){
                        int idtipoInimigo = DB.todosTiposInimigo
                            .FindIndex(inimigo => 
                                inimigo == tipoInimigo._aparencia)
                            + 1;
                        Personagem.MatarInimigo((EnumInimigos)idtipoInimigo);
                        Personagem.inventario.espada.nmrPuloAtaqueValido = 0;
                        acaoInimigo(true);
                        limpaLugarAntigoInimigo(posicoesInimigos, i);
                        break;

                    } else if(destino == "[]"){
                        Personagem.inventario.escudo._equipado = false;
                        acaoInimigo(true);
                        break;
                        
                    } else if(destino == "()"){
                        if (tipoInimigo._aparencia != "@@") {
                            limpaLugarAntigoInimigo(posicoesInimigos, i);
                        }
                        acaoInimigo();
                        Mapa.CheckMapaIsRenderizando();
                        MostrarMensagem.GameOver();
                        Environment.Exit(0);
                        
                    } else {
                        cloneAltPosicao.RemoveAt(idxAleatorio);
                    }
                };
            }
            Mapa.CheckMapaIsRenderizando();
        }
        public static async Task IntervaloMovimentoInimigo(){
            if (Game.FaseAtual._qntInimigosTipo1 > 0) {              
                await Task.Delay((int)(1500 * Game.dificuldade));
                MovimentacaoInimigo(tipoInimigo1);
                await IntervaloMovimentoInimigo();  
            }
        }
        public static async Task IntervaloMovimentoInimigo2(){
            if (Game.FaseAtual._qntInimigosTipo2 > 0) {
                await Task.Delay((int)(750 * Game.dificuldade));
                MovimentacaoInimigo(tipoInimigo2);
                await IntervaloMovimentoInimigo2();  
            }
        }
        public static async Task IntervaloMovimentoInimigo3(){
            if (Game.FaseAtual._qntInimigosTipo3 > 0) {
                await Task.Delay((int)(1200 * Game.dificuldade));
                MovimentacaoInimigo(tipoInimigo3);
                await IntervaloMovimentoInimigo3();  
            }
        }
        public static async Task IntervaloMovimentoInimigo5(){
            if (Game.FaseAtual._qntInimigosTipo5 > 0) {           
                await Task.Delay((int)(1500 * Game.dificuldade));
                MovimentacaoInimigo(tipoInimigo5);
                await IntervaloMovimentoInimigo5();  
            }
        }
    }
}