import processing.opengl.*;
import SimpleOpenNI.*;
int exercicioAtual = 0;

public class SkeletonDynamicsExercice
{
  SimpleOpenNI kinect;
  int[] listaDeExercicios;

  int userId;
  final int maximoProgresso = 200;
  
  public SkeletonDynamicsExercice(SimpleOpenNI context, int[] posicoes,boolean resetarPosicoes, int tempId){
    kinect = context;
    listaDeExercicios = posicoes;
    if(resetarPosicoes){
      exercicioAtual = 0;
    }
    userId = tempId;
  }
  
  float calcularExercicio(){
    float resultado = 0;
    if(kinect.isTrackingSkeleton(userId)){
       fill(0);
       text(nomesDosExercicios[listaDeExercicios[exercicioAtual]],750,550);
       switch(listaDeExercicios[exercicioAtual]){
        case 0:
          resultado = posicaoPi();
        break;
        case 1:
          resultado = posicaoPsi();
        break;
        case 2:
          resultado = posicaoNormal();
        break;
        case 3:
          resultado = posicaoDelta()*0.6 + posicaoPsi()*0.4;
        break;
          
       }
       if(resultado < 0){
        return 0;
       }
       else{
         return resultado;
       }
    }
    textSize(32);
    fill(0);
    return resultado;
  }
  
  private float posicaoPi(){
    PVector maoDireita = new PVector();
    PVector maoEsquerda = new PVector();
    float   angulo,anguloMapeado,distanciaVetorial,distanciaMapeada, resultado;
    
    
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_HAND,maoEsquerda);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_HAND,maoDireita);
    angulo = degrees(PVector.angleBetween(maoDireita,maoEsquerda));
    anguloMapeado = map(angulo,12,55,0,maximoProgresso*0.72);
    distanciaVetorial = euclidiana2D(maoDireita,maoEsquerda);
    distanciaMapeada = map(distanciaVetorial,400,1500,0,maximoProgresso*0.28);
    resultado = anguloMapeado + distanciaMapeada;
    //println(resultado);
    return resultado;
  }
  
  private float posicaoPsi(){
    PVector ombroDireito = new PVector();
    PVector ombroEsquerdo = new PVector();
    PVector cotoveloDireito = new PVector();
    PVector cotoveloEsquerdo = new PVector();
    PVector maoDireita = new PVector();
    PVector maoEsquerda = new PVector();
    PVector cabeca = new PVector();
    PVector quadrilDireito = new PVector();
    PVector quadrilEsquerdo = new PVector();
    
    float   anguloEsquerdo,anguloDireito, anguloMapeadoEsquerda,anguloMapeadoDireita, distanciaDireita, distanciaEsquerda,distanciaEsquerdaMapeada,distanciaDireitaMapeada;

    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_HAND,maoEsquerda);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_HAND,maoDireita);    
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_SHOULDER,ombroEsquerdo);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_SHOULDER,ombroDireito);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_ELBOW,cotoveloEsquerdo);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_ELBOW,cotoveloDireito);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_HEAD,cabeca);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_HIP,quadrilDireito);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_HIP,quadrilEsquerdo);
    
    distanciaDireita = euclidiana2D(maoDireita,quadrilDireito);
    distanciaEsquerda = euclidiana2D(maoEsquerda,quadrilEsquerdo);
    
    maoEsquerda.sub(cotoveloEsquerdo);
    maoDireita.sub(cotoveloDireito);
    cotoveloDireito.sub(ombroDireito);
    cotoveloEsquerdo.sub(ombroEsquerdo);
    anguloEsquerdo = degrees(PVector.angleBetween(maoEsquerda,cotoveloEsquerdo));
    anguloDireito = degrees(PVector.angleBetween(maoDireita,cotoveloDireito));
    distanciaDireitaMapeada = map(distanciaDireita,100,900,0,maximoProgresso*0.2);
    distanciaEsquerdaMapeada = map(distanciaEsquerda,100,900,0,maximoProgresso*0.2);
    anguloMapeadoEsquerda = customMap(anguloEsquerdo,15,90,0,maximoProgresso*0.30);
    anguloMapeadoDireita = customMap(anguloDireito,15,90,0,maximoProgresso*0.30);
    return distanciaDireitaMapeada + distanciaEsquerdaMapeada + anguloMapeadoEsquerda + anguloMapeadoDireita;
 
  }
  private float posicaoDelta(){
    PVector maoDireita = new PVector();
    PVector maoEsquerda = new PVector();
    PVector peDireito = new PVector();
    PVector peEsquerdo = new PVector();
    float anguloInferiorMapeado,anguloInferior;
    
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_HAND,maoEsquerda);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_HAND,maoDireita); 
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_FOOT,peEsquerdo);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_FOOT,peDireito);   

    anguloInferior = degrees(PVector.angleBetween(peDireito,peEsquerdo));
    anguloInferiorMapeado = map(anguloInferior,4,22,0,maximoProgresso);
    return anguloInferiorMapeado;
  }  
    
  private float posicaoNormal(){
    PVector quadrilDireito = new PVector();
    PVector quadrilEsquerdo = new PVector();
    PVector maoDireita = new PVector();
    PVector maoEsquerda = new PVector();
    float distanciaDireita, distanciaEsquerda,distanciaEsquerdaMapeada,distanciaDireitaMapeada;
    
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_HAND,maoEsquerda);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_HAND,maoDireita);    
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_RIGHT_HIP,quadrilDireito);
    this.kinect.getJointPositionSkeleton(userId,SimpleOpenNI.SKEL_LEFT_HIP,quadrilEsquerdo);    
    
    distanciaDireita = euclidiana2D(maoDireita,quadrilDireito);
    distanciaEsquerda = euclidiana2D(maoEsquerda,quadrilEsquerdo);
    distanciaDireitaMapeada = map(distanciaDireita,900,100,0,maximoProgresso*0.5);
    distanciaEsquerdaMapeada = map(distanciaEsquerda,900,100,0,maximoProgresso*0.5);
    
    return distanciaDireitaMapeada + distanciaEsquerdaMapeada;
  }
        
  float customMap(float entrada,float x1,float x2,float y1,float y2)
  {
    float result, mapa,aux;
    mapa = map(entrada,x1,x2,y1,y2);
    if(mapa > y2){
      aux = mapa - y2;
      mapa = mapa - aux;
    }
    return mapa;
  }
  
  float euclidiana2D(PVector vetor1, PVector vetor2){
   float diff_x, diff_y, diff_z; 
   
   diff_x = vetor1.x - vetor2.x;
   diff_y = vetor1.y - vetor2.y;
   
   return sqrt(pow(diff_x,2)+pow(diff_y,2));
    
  }
  float euclidiana3D(PVector point1, PVector point2)
  {
   float diff_x, diff_y, diff_z;    
   float distance;                  
 
   diff_x = point1.x - point2.x;
   diff_y = point1.y - point2.y;
   diff_z = point1.z - point2.z; 

   distance = sqrt(pow(diff_x,2)+pow(diff_y,2)+pow(diff_z,2));
 
   return distance;
  }   
}
