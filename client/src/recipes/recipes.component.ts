import { Component, OnInit } from '@angular/core';
import { RecipesService } from '../../src/app/services/recipes/recipes.service';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [],
  templateUrl: './recipes.component.html',
  styleUrl: './recipes.component.css'
})
export class RecipesComponent implements OnInit {

recipeList: any [] = []


constructor(private recipeSrv : RecipesService){

}

ngOnInit(): void {
  
}

getAllRecipes(){
  this.recipeSrv.getAllRecipes().subscribe((res:any) => {
    this.recipeList = res.data;
  })
}

}
