import { METHODS } from "http";

export const Constant = {
    APIEndPoint : 'https://localhost:7047/api/v1/recipes',
    METHODS: {
        GetAllRecipes: (params?: { name?: string, category?: string, pageNumber?: number, pageSize?: number }) => {
            let url = `${Constant.APIEndPoint}?`;

            if (params) {
                if (params.name) url += `name=${params.name}&`;
                if (params.category) url += `category=${params.category}&`;
                if (params.pageNumber) url += `pageNumber=${params.pageNumber}&`;
                if (params.pageSize) url += `pageSize=${params.pageSize}&`;
            }

            if (url.endsWith('&')) {
                url = url.slice(0, -1);
            }

            return url;
        }
    }
}