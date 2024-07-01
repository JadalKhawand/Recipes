import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Constant } from './constant/constant';

@Injectable({
  providedIn: 'root'
})
export class RecipesService {

  constructor(private http: HttpClient) { }

  getAllRecipes(){
    return this.http.get(Constant.METHODS.GetAllRecipes({name:'TEST',category:'test',pageNumber:1, pageSize: 1}));
  }
}
