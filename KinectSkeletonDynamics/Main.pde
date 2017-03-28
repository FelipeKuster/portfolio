import SimpleOpenNI.*;

SimpleOpenNI  context;

String[] nomesDosExercicios = {"Braços abertos","Posição Psi","Posição Normal","Posição Delta"};

int[] posicoes = {0,1,2,1,2,1,2,0,2,1,3,2,3,0,1,2};
boolean tarefaConcluida = false;
SkeletonDynamicsExercice exercicios;
TProgressBar  progress;
final int progressoAceitavel = 30;    // 30/200 = 15% de erro no exercicio

void setup()
{
  size(1024,768);
  context = new SimpleOpenNI(this);
  context.setMirror(true);
  this.progress=new TProgressBar(200,50,750,100);
  if(context.isInit() == false)
  {
     println("Não foi possível iniciar o programa. Talvez a camera esteja desconectada!"); 
     exit();
     return;  
  }
  
  // enable depthMap generation 
  context.enableDepth();
   
  // enable skeleton generation for all joints
  context.enableUser();
 
  background(200,0,0);

  stroke(0,0,255);
  strokeWeight(3);
  smooth(); 
}

void draw()
{
  
  context.update();
  background(255,255,255);
  
  int[] userList = context.getUsers();
  for(int i=0;i<userList.length;i++)
  {
    if(context.isTrackingSkeleton(userList[i]))
    {
      stroke(userClr[ (userList[i] - 1) % userClr.length ] );
      drawSkeleton(userList[i]);
    }      
  }
  if (userList.length > 0 && !tarefaConcluida)  {
      exercicios = new SkeletonDynamicsExercice(context,posicoes,false,userList[0]);
      float resultado = exercicios.calcularExercicio();
      if(resultado > 200)
      {
        resultado = 200;
      }
      boolean exercicioSucedido = this.progress.setValue(resultado);
      fill(0,0,255);
      text(exercicioAtual,750,25);
      if(!exercicioSucedido){
        fill(0,0,255);
        this.progress.draw();
      }
      else{
        fill(0,255,0);
        textSize(32);
        text("Exercício concluído",750,100); 
        //saveFrame("exercicio-"+exercicios.listaDeExercicios[exercicioAtual]+".png");
        exercicioAtual++;
        if(exercicioAtual>(exercicios.listaDeExercicios.length-1)){
          tarefaConcluida = true; 
          println(tarefaConcluida);
      }
    }
  }
  else if (!(userList.length>0)){
    textSize(32);
    fill(255,204,0);
    text("Carregando..",750,100);
  }
  else if(tarefaConcluida){
    textSize(32);
    fill(255,0,0);
    text("Tarefa concluída",750,100);
  }        
}


void onNewUser(SimpleOpenNI curContext, int userId)
{
  println("Novo usuário : userId: " + userId);
  println("\tIniciando desenho do esqueleto.");
  
  curContext.startTrackingSkeleton(userId);
}

void onLostUser(SimpleOpenNI curContext, int userId)
{
  println("Usuário desconectado - userId: " + userId);
}


void mousePressed()
{
  exit();
}
