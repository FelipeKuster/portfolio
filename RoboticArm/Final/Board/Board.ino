#include <QueueList.h>

int ENABLE = 53;
int pinled = 13;
int pinMotor_0 = 12;
int pinMotor_1 = 11;
int pinMotor_2 = 9;
int pinMotor_3 = 7;
int pinMotor_4 = 5;
int pinFeedback_0 = A0;
int pinFeedback_1 = A1;
int pinFeedback_2 = A2;
int pinFeedback_3 = A3;
int pROT0 = 14;
int pROT1 = 15;
int pROT2 = 16;
int pROT3 = 17;
int pROT4 = 30;

float Kp=10,Ki=5;
float Integrativos[] = {0,0,0,0,0,0,0,0}; //Motor 0_erro0(passado) até Motor 3_erro_0(passado) . Depois são os erros atuais.
int Motores[] = {12,11,9,7};
int ROTs[] = {14,15,16,17};
int Feedbacks[] = {A0,A1,A2,A3};
int Countdown[] = {0,0,0,0};
int Kps[] = {12,5,7,2};
int Setpoints[] = {247,716,661,514};
int Diferenciais[] = {5,15,5,5};
int posicoesBase[] = {247,336,421,505,584,677,775};
int posicoesOmbro[] = {716,493};
int posicoesCotovelo[] = {661,478};
int posicoesPulso[] = {514,462};
int incomingValue = -1;
int motorAtual, referenciaAtual;

QueueList<int> listaDeMovimentos;  //padrão:  1 elemento= numero do motor; 2 elemento = referencia

boolean holds[] = {false,false,false,false,false};
boolean flagDePosicao = true;
boolean garraAberta = true;
int j = 0; //contador

void setup()
{
  Serial.begin(9600);
  pinMode(ENABLE,OUTPUT);
  pinMode(pinled,OUTPUT);
  pinMode(pROT4,OUTPUT);
  pinMode(pROT3,OUTPUT);
  pinMode(pROT2,OUTPUT);
  pinMode(pROT1,OUTPUT);
  pinMode(pROT0,OUTPUT);
  pinMode(pinMotor_0,OUTPUT);
  pinMode(pinMotor_1,OUTPUT);
  pinMode(pinMotor_2,OUTPUT);
  pinMode(pinMotor_3,OUTPUT);
  pinMode(pinMotor_4,OUTPUT);
  pinMode(pinFeedback_0,INPUT);
  pinMode(pinFeedback_1,INPUT);
  pinMode(pinFeedback_2,INPUT);
  pinMode(pinFeedback_3,INPUT);
  digitalWrite(ENABLE,HIGH); 
  //preencherPosicaoInicial();
  preencherPosicaoBase(); 
  //pegar0();
  //preencherPosicaoBase();
  //moverBase();
}

void loop()
{
   int countDeLista;
   int incomingByte;
   int valor = 0;
   int j = 2;
   int i;
   if (Serial.available() > 1)
   {
    int fun = Serial.read();
    switch(fun)
     {
       case(0):
       {
          incomingByte = Serial.read();
          flagDePosicao = true;
          valor = incomingByte;
          listaDeMovimentos.push(fun);
          listaDeMovimentos.push(247 + valor*2.9333);
          break;
       }
       case(1):
       {
          incomingByte = Serial.read();
          flagDePosicao = true;
          valor = incomingByte;
          listaDeMovimentos.push(fun);
          listaDeMovimentos.push(-3.022*valor+780);
          break;
       }
       case(2):
       {
          incomingByte = Serial.read();
          flagDePosicao = true;
          valor = incomingByte;
          listaDeMovimentos.push(fun);
          listaDeMovimentos.push(-3.3167*valor+795);
          break;
       }
       case(3):
       {
          incomingByte = Serial.read();
          flagDePosicao = true;
          valor = incomingByte;
          listaDeMovimentos.push(fun);
          listaDeMovimentos.push(-2.305*valor+685);
          break;
       }
       case(4):
       {
          incomingByte = Serial.read();
          if(incomingByte == 0)
          {
            abrirGarra();
          }
          else
          {
            fecharGarra();
          }
          break;
       }
     }
   }
   /*
   int Input0 = analogRead(Feedbacks[0]);
   int Input1 = analogRead(Feedbacks[1]);
   int Input2 = analogRead(Feedbacks[2]);
   int Input3 = analogRead(Feedbacks[3]);
   Serial.print(Input0);
   Serial.print(" ");
   Serial.print(Input1);
   Serial.print(" ");
   Serial.print(Input2);
   Serial.print(" ");
   Serial.print(Input3);
   Serial.println("");*/
   

   for(i=0; i < 4;i++)
   {
     if(holds[i])
     {
       controlarMotor(i,Setpoints[i]);
     }
  }
  
  if(flagDePosicao)
   {
     countDeLista = listaDeMovimentos.count();
     if(!listaDeMovimentos.isEmpty())
     {
       motorAtual = listaDeMovimentos.pop();
       referenciaAtual = listaDeMovimentos.pop();
       if(motorAtual == 4)
       {
         if(referenciaAtual==0)
         {
           abrirGarra();
         }
         if(referenciaAtual==255)
         {
           fecharGarra();
         }
       }
       else
       {
         flagDePosicao = false;
         holds[motorAtual] = false;
         Setpoints[motorAtual] = referenciaAtual;
         controlarMotor(motorAtual,referenciaAtual);
       }
     }
     else
     {
       return; //movimento concluido
     }
   }
   else //assumindo que a lista é instalada com pelo menos uma posição. Caso contrário há erro de refencia
   {
     controlarMotor(motorAtual,Setpoints[motorAtual]);
   }
  
}
void controlarMotor(int index, int ref)
{
  int sp, Setpoint, Input;
  float Output, Erro;
  Input = analogRead(Feedbacks[index]);
  /*Serial.print(index);
  Serial.print(" ");
  Serial.print(ref-Input);
  Serial.print(" ");
  Serial.print(Input);
  Serial.println("");*/
  Erro = Kps[index]*(ref - Input);
  if(Erro>255)
  {
    Erro = 255;
  }
  else if(Erro<-255)
  {
    Erro = -255;
  }
  //Serial.println(Erro);
  if(ref-Input<=-Diferenciais[index])
  {
    j = 0;
    digitalWrite(ROTs[index],HIGH);
    analogWrite(Motores[index],Erro);
  }
  else if(ref-Input>=Diferenciais[index])
  {
    j = 0;
    digitalWrite(ROTs[index],LOW);
    analogWrite(Motores[index],Erro);
  }
  else
  {
    j++;
    digitalWrite(ROTs[index],HIGH);
    digitalWrite(Motores[index],HIGH);
  }
  if(holds[index] == true)
  {
    j = 0;
  }
  else if(j>=1)
  {
    holds[index] = true;
    flagDePosicao = true;
    j = 0;
  }
}

void abrirGarra()
{
  garraAberta = true;
  digitalWrite(pinMotor_4,LOW);
  digitalWrite(pROT4,HIGH);
}
void fecharGarra()
{
  garraAberta = false;
  digitalWrite(pinMotor_4,HIGH);
  digitalWrite(pROT4,LOW);
}

void preencherPosicaoInicial()//{247,500,726,470},  //0
{
   listaDeMovimentos.push(2);
   listaDeMovimentos.push(493);
   listaDeMovimentos.push(1);
   listaDeMovimentos.push(716);
   listaDeMovimentos.push(0);
   listaDeMovimentos.push(505);   
   listaDeMovimentos.push(2);
   listaDeMovimentos.push(661); 
   listaDeMovimentos.push(3);
   listaDeMovimentos.push(514);
   listaDeMovimentos.push(4);
   listaDeMovimentos.push(255);  
}
void preencherPosicaoBase()//{247,500,726,470},  //0
{
   listaDeMovimentos.push(2);
   listaDeMovimentos.push(500);
   listaDeMovimentos.push(0);
   listaDeMovimentos.push(247);
   listaDeMovimentos.push(0);
   listaDeMovimentos.push(505);
   listaDeMovimentos.push(0);
   listaDeMovimentos.push(775); 
   listaDeMovimentos.push(0);
   listaDeMovimentos.push(247);
   listaDeMovimentos.push(1);
   listaDeMovimentos.push(716);
   listaDeMovimentos.push(2);
   listaDeMovimentos.push(661);
   listaDeMovimentos.push(2);
   listaDeMovimentos.push(478);
   listaDeMovimentos.push(1);
   listaDeMovimentos.push(493);
}
void preencherPosicaoCotovelo()//{247,500,726,470},  //0
{
   listaDeMovimentos.push(2);
   listaDeMovimentos.push(661);
}
void pegar0()
{
  listaDeMovimentos.push(1);
  listaDeMovimentos.push(628);
  listaDeMovimentos.push(2);
  listaDeMovimentos.push(700);
  listaDeMovimentos.push(3);
  listaDeMovimentos.push(513);
  listaDeMovimentos.push(0);
  listaDeMovimentos.push(524);
  listaDeMovimentos.push(1);
  listaDeMovimentos.push(711);
} 


void moverBase()
{
  listaDeMovimentos.push(1);
  listaDeMovimentos.push(532);
  listaDeMovimentos.push(2);
  listaDeMovimentos.push(793);
  listaDeMovimentos.push(3);
  listaDeMovimentos.push(518);
  listaDeMovimentos.push(0);
  listaDeMovimentos.push(274);
  listaDeMovimentos.push(1);
  listaDeMovimentos.push(728);  
}

//545
//274
