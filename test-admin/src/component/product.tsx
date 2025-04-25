import { List, useRecordContext, Link, Datagrid, TextField, ImageField, NumberField, Create, Edit, SimpleForm, TextInput, NumberInput, ReferenceInput, SelectInput, EditButton, DeleteButton } from 'react-admin';
import { Link as RouterLink } from 'react-router-dom';

const CustomImageField = ({ source }: { source: string }) => {
    const record = useRecordContext();

    if (!record || !record.id) {
        return <span>No Image</span>;
    }

    const imageUrl = `https://localhost:7252/api/Product/${record.id}/image`;

    return (
        <RouterLink to={`/products/${record.id}/update-image`}>
            <img src={imageUrl} alt="Product" style={{ width: '100px', height: 'auto' }} />
        </RouterLink>
    );
};

const postFilters = [
    <TextInput key="search" source="search" label="Search" alwaysOn />,
    <ReferenceInput key="categoryId" source="categoryId" reference="categories" label="Category">
        <SelectInput optionText="categoryName" />
    </ReferenceInput>
];

export const ProductList = () => (
    <List filters={postFilters}>
        <Datagrid rowClick={false}>
            <TextField source="id" label="Product ID" />
            <TextField source="name" label="Product Name" />
            <TextField source="categoryName" label="Category Name" />
            <CustomImageField source="image" />
            <TextField source="description" label="Description" />  
            <TextField source="quantity" label="Quantity" />
            <TextField source="discount" label="Discount" />
            <TextField source="specialPrice" label="SpecialPrice" />

            <NumberField source="price" label="Price" />
            <EditButton />
            <DeleteButton />
        </Datagrid>
    </List>
);

export const ProductCreate = () => (
    <Create>
        <SimpleForm>
            <TextInput source="name" label="Product Name (at least 3 characters)" />
            <TextInput source="description" multiline rows={5} label="Description (at least 6 characters)" />
            <NumberInput source="price" label="Price" />
            <NumberInput source="quantity" label="Quantity" />
            <NumberInput source="discount" label="Discount" />
            <NumberInput source="specialPrice" label="SpecialPrice" />
            <ReferenceInput source="CategoryId" reference="category" label="Category">
                <SelectInput optionText="id" />
            </ReferenceInput>
            <TextInput source="image" label="Image URL" />
        </SimpleForm>
    </Create>
);

export const ProductEdit = () => (
    <Edit>
        <SimpleForm>
            <TextInput source="id" disabled label="Product ID" />
            <ReferenceInput source="CategoryId" reference="category" label="Category">
                <SelectInput optionText="id" />
            </ReferenceInput>
            <TextInput source="name" label="Product Name" />
            <TextInput source="image" label="Image URL" />
            <TextInput source="description" multiline rows={5} label="Description" />
            <NumberInput source="price" label="Price" />
            <NumberInput source="quantity" label="Quantity" />
            <NumberInput source="discount" label="Discount" />
            <NumberInput source="specialPrice" label="SpecialPrice" />
        </SimpleForm>
    </Edit>
);