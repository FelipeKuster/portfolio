public class TProgressBar
{
   
  private int x;
   
  private int y;
   
  private int maxWidth;
   
  private int heightProgress;
     
  private float value;
   
  private PFont font;
   
  private color backgroundColor;
   
  private color progressColor;
   
  private int sizeFont;
   
  public TProgressBar(int maxWidth,int heightProgress,int x, int y)
  {
     
    this.maxWidth=maxWidth;
     
    this.heightProgress=heightProgress;
     
    this.x=x;
     
    this.y=y;
     
    this.value=x;
    
    this.backgroundColor=color(182,205,250,120);
    
    this.progressColor=color(0,255,0);
     
    this.sizeFont=30;
     
    this.font=loadFont("BodoniMT-48.vlw");
    
    textFont(this.font);
     
    textSize(this.sizeFont);
    
    smooth();
  }
   
  public void setMaxWidth(int maxWidth)
  {
    this.maxWidth=maxWidth;
  }
   
  public void setHeightProgress(int heightProgress)
  {
    this.heightProgress=heightProgress;
  }
   
  public void setX(int x)
  {
    this.x=x;
  }
   
  public void setY(int y)
  {
    this.y=y;
  }
   
  public void setBackgroundColor(color backgroundColor)
  {
    this.backgroundColor=backgroundColor;
  }
   
   
  public void setProgressColor(color progressColor)
  {
    this.progressColor=progressColor;
  }
   
   
  public void setFontSize(int fontSize)
  {
    this.sizeFont=sizeFont;
  }
   
  public int percentage()
  {
   return (int)( (  this.value * 100 )/this.maxWidth ) ; 
  }
   
   
  public boolean setValue(float value)
  {
    if( value < this.maxWidth-progressoAceitavel)
     {
       this.value=value;
     }
    else if(value <= this.maxWidth)
    {
      this.value = value;
      return true;
    }
     else
    {
       println("Progresso máximo atingido.");
       return true;
    }
    return false;
  }
   
 public void draw()
 {
 
  pushStyle();
     
   textSize(this.sizeFont);
         
    strokeCap(SQUARE);
  
    stroke(this.backgroundColor);
  
    strokeWeight(heightProgress);
  
    line(this.x,this.y,this.x+this.maxWidth,this.y);
  
    stroke(this.progressColor);
  
    strokeWeight(heightProgress);
  
    line(this.x,this.y,this.x+this.value,this.y);
     
    textAlign(CENTER); 
  
    text("Precisão: "+percentage() + " %",this.x+this.maxWidth/2,this.y-30);
   
  popStyle();
 }
   
}

