import { MongooseManuscriptRepository } from "@/repositories/MongooseManuscriptRepository";
import { MongooseWikiEntryRepository } from "@/repositories/MongooseWikiEntryRepository";
import { Neo4jGraphRepository } from "@/repositories/Neo4jGraphRepository";
import type { IManuscriptRepository } from "@/interfaces/repositories/IManuscriptRepository";
import type { IWikiEntryRepository } from "@/interfaces/repositories/IWikiEntryRepository";
import type { IGraphRepository } from "@/interfaces/repositories/IGraphRepository";

export interface IContainer {
  manuscriptRepo: IManuscriptRepository;
  wikiRepo: IWikiEntryRepository;
  graphRepo: IGraphRepository;
}

const defaultContainer: IContainer = {
  manuscriptRepo: new MongooseManuscriptRepository(),
  wikiRepo: new MongooseWikiEntryRepository(),
  graphRepo: new Neo4jGraphRepository(),
};

export let container: IContainer = defaultContainer;

export const setContainer = (overrides: Partial<IContainer>): void => {
  container = { ...defaultContainer, ...overrides };
};
