import axios from 'axios';
import { CreateParams, CreateResult, DataProvider, DeleteManyParams, DeleteManyResult, DeleteParams, DeleteResult, GetManyParams, GetManyReferenceParams, GetManyReferenceResult, GetManyResult, GetOneParams, GetOneResult, Identifier, QueryFunctionContext, RaRecord, UpdateManyParams, UpdateManyResult, UpdateParams, UpdateResult } from 'react-admin';

const apiUrl = 'https://localhost:7252/api';

const httpClient = {
    get: (url: string) => {
        const token = localStorage.getItem('jwt-token');
        return axios.get(url, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            withCredentials: true,
        }).then(response => ({ json: response.data }))
          .catch(error => {
              console.error('API request failed:', error);
              throw error;
          });
    },

    post: (url: string, data: any) => {
        const token = localStorage.getItem('jwt-token');
        console.log('jwt-token:', token);
        return axios.post(url, data, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            withCredentials: true,
        }).then(response => ({ json: response.data }))
          .catch(error => {
              console.error('API request failed:', error);
              throw error;
          });
    },

    put: (url: string, data: any) => {
        const token = localStorage.getItem('jwt-token');

        return axios.put(url, data, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            withCredentials: true,
        }).then(response => ({ json: response.data }))
          .catch(error => {
              console.error('API request failed:', error);
              throw error;
          });
    },

    delete: (url: string, p0: { data: { ids: any[]; }; }) => {
        const token = localStorage.getItem('jwt-token');

        return axios.delete(url, {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            withCredentials: true,
        }).then(response => ({ json: response.data }))
          .catch(error => {
              console.error('API request failed:', error);
              throw error;
          });
    },
};

export const dataProvider: DataProvider = {
    getList: async (resource: string, { pagination = {}, filter = {} }) => {
        const idFieldMapping: { [key: string]: string } = {
            products: 'productId',
            categories: 'categoryId',
            carts: 'cartId',
            users: 'userId',
            orders: 'orderId',
        };

        const idField = idFieldMapping[resource] || 'id';
        const { page = 1, perPage = 10 } = pagination;

        // Xây dựng URL mà không có _sort và _order
        const query = {
            ...filter,
            _page: page,
            _limit: perPage,
        };
        const url = `${apiUrl}/${resource}?${new URLSearchParams(query).toString()}`;

        try {
            const result = await httpClient.get(url);
            const json = result.json;

            const items = Array.isArray(json)
                ? json
                : Array.isArray(json.content)
                ? json.content
                : [];

            const data = items.map((item: any) => ({
                id: item[idField],
                ...item,
            }));

            const total = json.totalElements ?? data.length;

            return { data, total };
        } catch (error) {
            console.error("Error fetching list:", error);
            return { data: [], total: 0 };
        }
    },
    
    delete: async <RecordType extends RaRecord = any>(resource: string, params: DeleteParams<RecordType>): Promise<DeleteResult<RecordType>> => {
        try {
            // Construct the URL for the DELETE request
            const url = `${apiUrl}/${resource}/${params.id}`;
    
            // Perform the DELETE request
            await httpClient.delete(url, {
                data: {
                    ids: [params.id], // Assuming you want to pass the ID of the item to delete
                },
            });
    
            // Return an empty result
            return {
                data: params.previousData as RecordType,
            };
        } catch (error) {
            console.error('API request failed:', error);
            throw new Error('Error deleting record');
        }
    },
    deleteMany: async <RecordType extends RaRecord = any>(
        resource: string,
        params: DeleteManyParams
    ): Promise<DeleteManyResult<RecordType>> => {
        const { ids } = params;
    
        try {
            // Create an array of promises for each delete request
            const deletePromises = ids.map(id => {
                const url = `${apiUrl}/${resource}/${id}`;
                return httpClient.delete(url, {
                    data: {
                        ids: [id], // Sending the ID as part of the request body, if needed
                    },
                });
            });
    
            // Execute all delete requests in parallel
            await Promise.all(deletePromises);
    
            // Return the IDs of the deleted records
            return {
                data: ids,
            };
        } catch (error) {
            console.error('API request failed:', error);
            throw new Error('Error deleting records');
        }
    },
    
    getManyReference: function <RecordType extends RaRecord = any>(resource: string, params: GetManyReferenceParams & QueryFunctionContext): Promise<GetManyReferenceResult<RecordType>> {
        throw new Error('Function not implemented.');
    },
    updateMany: function <RecordType extends RaRecord = any>(resource: string, params: UpdateManyParams): Promise<UpdateManyResult<RecordType>> {
        throw new Error('Function not implemented.');
    },
    create: async (resource: string, params: CreateParams): Promise<CreateResult> => {
        try {
            let url: string;
            if (resource === "product") {
                url = `${apiUrl}/product`; // Sửa endpoint thành /product
                delete params.data.categoryId; // Xóa categoryId nếu không cần thiết
                params.data.image = 'default.png'; // Gán giá trị mặc định cho image
            } else {
                url = `${apiUrl}/${resource}`;
            }
            const { data } = params;
            const result = await httpClient.post(url, data);
            return { data: { ...data, id: result.json.id } };
        } catch (error) {
            console.error("Error creating resource:", error);
            throw error; // Hoặc xử lý lỗi tùy theo nhu cầu của bạn
        }
    },
    update: async (resource: string, params: UpdateParams): Promise<UpdateResult> => {
        
        const { data } = params;
        //console.log("params", params);
        let url: string;
        if (resource === "users") {
            url = `${apiUrl}/${resource}/role/${params.id}/${params.data.rolesUpdate[0]}`;
        } else {
            url = `${apiUrl}/${resource}/${params.id}`;
        }
        //console.log("paramsurl", url);
        // Perform the PUT request to update the resource
        const result = await httpClient.put(url, data);
    
        // Assuming the API response contains the updated data with the correct 'id'
        const updatedData = {
            id: params.id,  // Ensure 'id' is included in the response
            ...result.json
        };
    
        return { data: updatedData };
    },
    getOne: async (resource: string, params: GetOneParams): Promise<GetOneResult> => {
        let url: string;
        if (resource === "carts") {
            url = `${apiUrl}/users/${localStorage.getItem('username')}/${resource}/${params.id}`;
        } else {
            url = `${apiUrl}/${resource}/${params.id}`;
        }
        const result = await httpClient.get(url);

        const idFieldMapping: { [key: string]: string } = {
            products: 'productId',
            categories: 'categoryId',
            carts: 'cartId',
        };
        const idField = idFieldMapping[resource] || 'id';

        let data;
        if (resource === "carts") {
            data = {
                id: result.json[idField],
                totalPrice: result.json.totalPrice,
                products: result.json.products.map((product: any) => ({
                    id: product.productId,
                    productName: product.productName,
                    image: product.productId
                        ? `https://localhost:7252/api/Product/${product.productId}/image`
                        : null,
                    description: product.description,
                    quantity: product.quantity,
                    price: product.price,
                    discount: product.discount,
                    specialPrice: product.specialPrice,
                    category: product.category
                        ? {
                              id: product.category.categoryId,
                              name: product.category.categoryName,
                          }
                        : null,
                })),
            };
        } else {
            data = {
                id: result.json[idField],
                ...result.json,
                image: result.json[idField]
                    ? `https://localhost:7252/api/Product/${result.json[idField]}/image`
                    : null,
            };
        }
        return { data };
    },
        
    getMany: async (resource: string, params: GetManyParams): Promise<GetManyResult> => {
        const idFieldMapping: { [key: string]: string } = {
            products: 'productId',
            categories: 'categoryId',
        };
        console.log('Request resource:', resource);

        const idField = idFieldMapping[resource] || 'id';

        // Construct the URL with the selected IDs
        const ids = params.ids.join(',');
        let url: string;
        if (resource === "products") {
            url = `${apiUrl}/category/${ids}/${resource}`;
        } else {
            url = `${apiUrl}/${resource}`;
        }
        console.log('Request URL getMany:', url);

        try {
            // Perform the GET request
            const result = await httpClient.get(url);

            // Validate the response structure
            if (!result.json || !Array.isArray(result.json.content)) {
                console.error('Unexpected API response structure:', result.json);
                throw new Error('Invalid API response structure');
            }

            // Map the results to include the correct 'id' field and image URL
            const data = result.json.content.map((item: any) => ({
                id: item[idField],
                ...item,
                image: item[idField]
                    ? `https://localhost:7252/api/Product/${item[idField]}/image`
                    : null,
            }));

            return { data };
        } catch (error) {
            console.error('Error in getMany:', error);
            return { data: [] }; // Return an empty array in case of an error
        }
    },
    
};


